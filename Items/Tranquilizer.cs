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

namespace CustomItems.Items;

[CustomItem(ItemType.GunCOM15)]
public class Tranquilizer : CustomWeapon {
  public override string Name { get; set; } = "Tranquilizer";
  public override uint Id { get; set; } = 1290;

  public override string Description { get; set; } =
    "A gun that temporarily tranquilizes entities; might be unreliable.";

  public override float Weight { get; set; } = 1f;
  public override float Damage { get; set; } = 1f;
  public override byte ClipSize { get; set; } = 3;

  [Description("Whether the tranquilizer is effective on SCP-173.")]
  public bool EffectiveOn173 { get; set; } = false;

  [Description("The effectiveness of tranquilizer on SCPs in decimal percentage.")]
  public float ScpChance { get; set; } = 0.5f;

  [Description("The effectiveness of tranquilizer on humans in decimal percentage.")]
  public float HumanChance { get; set; } = 0.75f;
  
  [Description("Resistance to remove from the chance after being shot.")]
  public float Resistance { get; set; } = 0.05f;

  [YamlIgnore] private readonly Random _rng = new();
  [YamlIgnore] private readonly Dictionary<uint, float> _resistances = new();

  public override SpawnProperties? SpawnProperties { get; set; } = new() {
    Limit = 1,
    RoomSpawnPoints = [
      new RoomSpawnPoint() { Room = RoomType.LczCafe, Chance = 25 },
      new RoomSpawnPoint() { Room = RoomType.LczGlassBox, Chance = 25 },
      new RoomSpawnPoint() { Room = RoomType.LczPlants, Chance = 75 }
    ]
  };

  protected override void OnShot(ShotEventArgs ev) {
    if (ev.Target == null) return;
    
    var rand = _rng.NextDouble();
    var tResistance = _resistances.GetValueOrDefault(ev.Target.NetId, 0);
    var effective = ev.Target.IsScp ? rand < (ScpChance - tResistance) : rand < (HumanChance - tResistance);
    if ((ev.Target.Role == RoleTypeId.Scp173 && !EffectiveOn173) || !effective) {
      base.OnShot(ev);
      return;
    }

    var lift = ev.Player.Lift;
    ev.Target.Scale = Vector3.zero;
    _resistances[ev.Target.NetId] = tResistance + Resistance;
    ev.Target.EnableEffect(EffectType.Ensnared, byte.MaxValue);
    ev.Target.EnableEffect(EffectType.Flashed, byte.MaxValue);
    ev.Target.EnableEffect(EffectType.Deafened, byte.MaxValue);
    Ragdoll? ragdoll = null;
    if (ev.Target.Role != RoleTypeId.Scp106) ragdoll = Ragdoll.CreateAndSpawn(ev.Target.Role.Type, ev.Target.DisplayNickname, new CustomReasonDamageHandler("Tranquilized."), ev.Target.Position, ev.Target.Rotation, ev.Target);
    if (ev.Target.Role == RoleTypeId.Scp096) {
      var crybaby = (Scp096Role)ev.Target.Role;
      if (crybaby.RageState is Scp096RageState.Enraged or Scp096RageState.Distressed) crybaby.RageManager.ServerEndEnrage();
    }

    Timing.CallDelayed(5, () => {
      ev.Target.DisableEffect(EffectType.Ensnared);
      ev.Target.DisableEffect(EffectType.Flashed);
      ev.Target.DisableEffect(EffectType.Deafened);
      if (lift != null) {
        ev.Target.Teleport(lift.Position + Vector3.up * 2f);
      }
      else {
        ev.Target.Teleport(ev.Target.Position + Vector3.up * 2f);
      }
      ev.Target.Scale = Vector3.one;
      ragdoll?.Destroy();
    });

    base.OnShot(ev);
  }
}