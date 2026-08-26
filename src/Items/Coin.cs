using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using ExtendedItems.Types;
using LabApi.Features.Wrappers;
using MEC;
using PlayerStatsSystem;
using UnityEngine;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Ragdoll = Exiled.API.Features.Ragdoll;

namespace ExtendedItems.Items;

[CustomItem(ItemType.Coin)]
public class Coin : CustomItem
{
    public override string Name { get; set; } = "SCP-1289";
    public override uint Id { get; set; } = 4;
    public override string Description { get; set; } = "<i>\"What's the most you ever lost on a coin toss?\"</i>";
    public override float Weight { get; set; } = 1f;
    
    private CoroutineHandle _handler;
    private LightSourceToy? _lightSourceToy;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 0,
        RoomSpawnPoints =
        [
            new RoomSpawnPoint { Room = RoomType.LczCafe, Chance = 50 },
            new RoomSpawnPoint { Room = RoomType.LczToilets, Chance = 50 },
            new RoomSpawnPoint { Room = RoomType.HczArmory, Chance = 50 }
        ],
        DynamicSpawnPoints =
        [
            new DynamicSpawnPoint { Location = SpawnLocationType.InsideEscapePrimary, Chance = 50 }
        ]
    };

    ~Coin()
    {
        if (_handler.IsValid) Timing.KillCoroutines(_handler);
        _lightSourceToy = null;
        _handler = default;
    }

    [Description("Effects to give if coin landed on heads.")]
    private static CoinEffect[] Effects =>
    [
        new() { Type = EffectType.DamageReduction, Duration = 15, Intensity = 20 },
        new() { Type = EffectType.RainbowTaste, Duration = 15, Intensity = byte.MaxValue },
        new() { Type = EffectType.Invigorated, Duration = 15, Intensity = byte.MaxValue },
        new() { Type = EffectType.MovementBoost, Duration = 15, Intensity = 25 }
    ];

    protected override void SubscribeEvents()
    {
        PlayerEvents.FlippingCoin += OnFlippingCoin;
        PlayerEvents.ChangingRole += OnRoleChanging;
        PlayerEvents.ItemAdded += ItemAdded;

        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        PlayerEvents.FlippingCoin -= OnFlippingCoin;
        PlayerEvents.ChangingRole -= OnRoleChanging;
        PlayerEvents.ItemAdded -= ItemAdded;

        base.UnsubscribeEvents();
    }

    private void ItemAdded(ItemAddedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        Utils.RemoveLight(ev.Item.CreatePickup(ev.Player.Position, ev.Player.Rotation, false));
    }

    private void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        if (ev.IsAllowed)
        {
            Timing.CallDelayed(2f,
                () =>
                {
                    if (ev.IsTails)
                    {
                        if (ev.Player.ActiveEffects.Any(targetActiveEffect =>
                                targetActiveEffect.name == nameof(EffectType.AntiScp207)))
                        {
                            Log.Debug($"{ev.Player.Nickname} had Anti-207");
                            ev.Player.Explode(ProjectileType.FragGrenade, ev.Player);
                            ev.Player.DisableEffect(EffectType.AntiScp207);
                        }
                        else
                        {
                            Log.Debug("Coin Landed on tails");
                            ev.Player.Scale = Vector3.zero;
                            Log.Debug($"Scaling {ev.Player.Nickname} to zero");

                            // ReSharper disable once InconsistentNaming
                            var _temp = Plugin.Instance?.Config.LoseCauses.RandomItem() ??
                                        "<Error: Coin Reason Not Found>";

                            var cause = _temp.Any(char.IsWhiteSpace)
                                ? $"the words {_temp} are etched into the scalp"
                                : $"the word {_temp} is etched in the scalp";
                            Ragdoll.CreateAndSpawn(ev.Player.Role.Type,
                                ev.Player.DisplayNickname,
                                new CustomReasonDamageHandler(cause),
                                ev.Player.Position,
                                ev.Player.Rotation,
                                ev.Player);

                            ev.Player.IsGodModeEnabled = false;

                            Log.Debug("Spawning Grenade");
                            Utils.Exploding(ev.Player);
                            ev.Player.Kill(cause);
                            Utils.PlayersUsedCoin[ev.Player.Id] = ev.Player;
                        }
                    }
                    else
                    {
                        ev.Player.ShowHint($"{Plugin.Instance?.Config.WinHints.RandomItem()}");

                        if (ev.Player.Health + ev.Player.ArtificialHealth < ev.Player.MaxHealth / 2)
                            ev.Player.AddRegeneration(duration: 10, rate: 5);
                        else
                            Effects.ForEach(effect =>
                            {
                                ev.Player.EnableEffect(effect.Type,
                                    effect.Intensity,
                                    effect.Duration,
                                    true);
                            });
                    }
                });
            ev.IsAllowed = false;

            Timing.CallDelayed(5f, () => { ev.IsAllowed = true; });
        }
        else
        {
            ev.Player.ShowHint("Wait", 1f);
        }
    }

    // protected override void OnDroppingItem(DroppingItemEventArgs ev)
    // {
    //     if (!Check(ev.Item)) return;
    //     CoinHintHandler(ev.Item.CreatePickup(ev.Player.Position, ev.Player.Rotation, false), out _handler);
    //     base.OnDroppingItem(ev);
    // }
    //
    // protected override void OnAcquired(Player player, Item item, bool displayMessage)
    // {
    //     if (!Check(item)) return;
    //     if (_handler.IsValid) Timing.KillCoroutines(_handler);
    //     _lightSourceToy.Destroy();
    //     base.OnAcquired(player, item, displayMessage);
    // }

    private void OnRoleChanging(ChangingRoleEventArgs ev)
    {
        ev.Player.Scale = Vector3.one;
        ev.Player.DisableAllEffects();
    }

    private static IEnumerator<float> CoinCoroutine(Pickup pickup)
    {
        yield return Timing.WaitForSeconds(Plugin.Instance.Config.SCP1289Timer);
        var intensity = 0;
        var light = LightSourceToy.Create(pickup.Position, pickup.Rotation, pickup.Transform, false);
        light.Color = Utils.ParseConfigColor();
        light.Range = Plugin.Instance.Config.SCP1289LightRange;
        light.Type = LightType.Tube;
        light.ShadowStrength = .5f;

        while (intensity != 100)
        {
            light.Intensity = intensity;
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.TimeToFullGlow / 100);
            intensity++;
        }
    }

    private static void CoinHintHandler(Pickup pickup, out CoroutineHandle handle)
    {
        var glowCoroutine = Timing.RunCoroutine(CoinCoroutine(pickup));
        handle = glowCoroutine;
    }
}