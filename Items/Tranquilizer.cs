using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;
using Random = System.Random;
using Scp096Role = Exiled.API.Features.Roles.Scp096Role;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace ExtendedItems.Items;

[CustomItem(ItemType.GunCOM15)]
public class Tranquilizer : CustomWeapon
{
    [YamlIgnore] private readonly Dictionary<uint, float> _resistances = [];


    [YamlIgnore] private readonly Random _rng = new();
    public override string Name { get; set; } = "Tranquilizer";
    public override uint Id { get; set; } = 1;

    public override string Description { get; set; } =
        "A gun that temporarily tranquilizes entities; might be unreliable.";

    public override float Weight { get; set; } = 1f;
    public override float Damage { get; set; } = 1f;
    public override byte ClipSize { get; set; } = 3;

    [Description("Whether the tranquilizer is effective on SCP-173.")]
    private bool EffectiveOn173 { get; } = false;

    [Description("The effectiveness of tranquilizer on SCPs in decimal percentage.")]
    private float ScpChance { get; } = 0.5f;

    [Description("The effectiveness of tranquilizer on humans in decimal percentage.")]
    private float HumanChance { get; } = 0.75f;

    [Description("Resistance to remove from the chance after being shot.")]
    private float Resistance { get; } = 0.05f;


    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        RoomSpawnPoints =
        [
            new RoomSpawnPoint { Room = RoomType.LczCafe, Chance = 25 },
            new RoomSpawnPoint { Room = RoomType.LczGlassBox, Chance = 25 },
            new RoomSpawnPoint { Room = RoomType.LczPlants, Chance = 75 }
        ]
    };

    protected override void SubscribeEvents()
    {
        PlayerEvents.ChangingRole += OnChangingRole;
        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        PlayerEvents.ChangingRole -= OnChangingRole;
        base.UnsubscribeEvents();
    }

    private void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (!_resistances.ContainsKey(ev.Player.NetId)) return;
        _resistances[ev.Player.NetId] = 0;
    }

    protected override void OnShot(ShotEventArgs ev)
    {
        if (ev.Target == null || Plugin.Instance == null) return;
        if (ev.Target.IsTutorial && !Plugin.Instance.Config.EffectiveOnTutorials) return;

        if (!Utils.HasEffect(ev.Target, EffectType.Invigorated)) return;


        var rand = _rng.NextDouble();
        _resistances.TryGetValue(ev.Target.NetId, out var tResistance);

        var effective = ev.Target.IsScp ? rand < ScpChance - tResistance : rand < HumanChance - tResistance;

        if ((ev.Target.Role == RoleTypeId.Scp173 && !EffectiveOn173) || !effective)
        {
            base.OnShot(ev);
            return;
        }

        var lift = ev.Player.Lift;

        ev.Target.Scale = Vector3.zero;
        _resistances[ev.Target.NetId] = tResistance + Resistance;

        ev.Target.EnableEffect(EffectType.Ensnared, byte.MaxValue);
        ev.Target.EnableEffect(EffectType.Flashed, byte.MaxValue);
        ev.Target.EnableEffect(EffectType.Deafened, byte.MaxValue);
        ev.Target.EnableEffect(EffectType.DamageReduction, 60);

        Ragdoll? ragdoll = null;

        if (ev.Target.IsHuman)
        {
            ev.Target.CurrentItem = null;
            ev.Target.EnableEffect(EffectType.AmnesiaItems, byte.MaxValue);
        }
        else
        {
            if (ev.Target.Role != RoleTypeId.Scp106)
                ragdoll = Ragdoll.CreateAndSpawn(ev.Target.Role.Type, ev.Target.DisplayNickname,
                    new CustomReasonDamageHandler("Tranquilized."), ev.Target.Position, ev.Target.Rotation,
                    ev.Target);

            if (ev.Target.Role == RoleTypeId.Scp096)
            {
                var crybaby = (Scp096Role)ev.Target.Role;
                if (crybaby.RageState is Scp096RageState.Enraged or Scp096RageState.Distressed)
                    crybaby.RageManager.ServerEndEnrage();
            }
        }

        Timing.CallDelayed(5, () =>
        {
            ev.Target.Inventory.enabled = true;
            ev.Target.IsGodModeEnabled = false;
            ev.Target.DisableEffect(EffectType.Ensnared);
            ev.Target.DisableEffect(EffectType.Flashed);
            ev.Target.DisableEffect(EffectType.Deafened);
            ev.Target.DisableEffect(EffectType.AmnesiaItems);
            ev.Target.DisableEffect(EffectType.DamageReduction);

            if (lift != null) ev.Target.Teleport(lift.Position + Vector3.up * 2f);
            else ev.Target.Teleport(ev.Target.Position + Vector3.up * 2f);
            ev.Target.Scale = Vector3.one;
            ragdoll?.Destroy();
        });

        base.OnShot(ev);
    }
}