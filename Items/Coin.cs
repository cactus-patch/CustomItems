using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using ExtendedItems.Types;
using MEC;
using PlayerStatsSystem;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.Coin)]
    public class Coin : CustomItem
    {
        public override string Name { get; set; } = "SCP-1289";
        public override uint Id { get; set; } = 4;
        public override string Description { get; set; } = "<i>\"What's the most you ever lost on a coin toss?\"</i>";
        public override float Weight { get; set; } = 1f;

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            RoomSpawnPoints =
            [
                new RoomSpawnPoint { Room = RoomType.LczCafe, Chance = 50, },
                new RoomSpawnPoint { Room = RoomType.LczToilets, Chance = 50, },
                new RoomSpawnPoint { Room = RoomType.HczArmory, Chance = 50, },
            ],
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint { Location = SpawnLocationType.InsideEscapePrimary, Chance = 50, },
            ],
        };

        [Description("Effects to give if coin landed on heads.")]
        private static CoinEffect[] Effects =>
        [
            new() { Type = EffectType.DamageReduction, Duration = 15, Intensity = 75, },
            new() { Type = EffectType.RainbowTaste, Duration = 15, Intensity = byte.MaxValue, },
            new() { Type = EffectType.Invigorated, Duration = 15, Intensity = byte.MaxValue, },
            new() { Type = EffectType.MovementBoost, Duration = 15, Intensity = 75, },
        ];

        protected override void SubscribeEvents()
        {
            PlayerEvents.FlippingCoin += OnFlippingCoin;
            PlayerEvents.ChangingRole += OnRoleChanging;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvents.FlippingCoin -= OnFlippingCoin;
            PlayerEvents.ChangingRole -= OnRoleChanging;

            base.UnsubscribeEvents();
        }

        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            Timing.CallDelayed(2f,
                () =>
                {
                    if (ev.IsTails)
                    {
                        if (ev.Player.ActiveEffects.Any(targetActiveEffect => targetActiveEffect.name == nameof(EffectType.AntiScp207)))
                        {
                            Log.Debug($"{ev.Player.Nickname} had Anti-207");
                            ev.Player.Explode(ProjectileType.FragGrenade, ev.Player);
                        }
                        else
                        {
                            Log.Debug("Coin Landed on tails");
                            ev.Player.Scale = Vector3.zero;
                            Log.Debug($"Scaling {ev.Player.Nickname} to zero");

                            // ReSharper disable once InconsistentNaming
                            var _temp = Plugin.Instance?.Config.LoseCauses.RandomItem() ??
                                        "<Error: Coin Reason Not Found>";
                            
                            string cause = _temp.Any(char.IsWhiteSpace) ? $"the words {_temp} are etched into the scalp" : $"the word {_temp} is etched in the scalp";
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
                        }
                    }
                    else
                    {
                        ev.Player.ShowHint($"{Plugin.Instance?.Config.WinHints.RandomItem()}");

                        if (ev.Player.Health + ev.Player.ArtificialHealth < ev.Player.MaxHealth / 2)
                        {
                            ev.Player.AddRegeneration(duration: 10, rate: 5);
                        }
                        else
                        {
                            Effects.ForEach(effect =>
                            {
                                ev.Player.EnableEffect(effect.Type,
                                    effect.Intensity,
                                    effect.Duration,
                                    true);
                            });
                        }
                    }
                });
        }

        private static void OnRoleChanging(ChangingRoleEventArgs ev)
        {
            ev.Player.Scale = Vector3.one;
            ev.Player.DisableAllEffects();
        }
    }
}