using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using ExtendedItems.Types;
using MEC;
using System.ComponentModel;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.Coin)]
    public class Coin : CustomItem
    {
        public override string Name { get; set; } = "SCP-1289";
        public override uint Id { get; set; } = 804;
        public override string Description { get; set; } = "<i>\"What's the most you ever lost on a coin toss?\"</i>";
        public override float Weight { get; set; } = 1f;

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            RoomSpawnPoints =
            [
                new RoomSpawnPoint() { Room = RoomType.LczCafe, Chance = 50 },
                new RoomSpawnPoint() { Room = RoomType.LczToilets, Chance = 50 },
                new RoomSpawnPoint() { Room = RoomType.HczArmory, Chance = 50 }
            ],
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint() { Location = SpawnLocationType.InsideEscapePrimary, Chance = 50 }
            ]
        };

        [Description("Effects to give if coin landed on heads.")]
        public CoinEffect[] Effects { get; set; } =
        [
            new() { Type = EffectType.DamageReduction, Duration = 15, Intensity = 75 },
            new() { Type = EffectType.RainbowTaste, Duration = 15, Intensity = byte.MaxValue },
            new() { Type = EffectType.Invigorated, Duration = 15, Intensity = byte.MaxValue },
            new() { Type = EffectType.MovementBoost, Duration = 15, Intensity = 75 }
        ];

        protected override void SubscribeEvents()
        {
            PlayerEvents.FlippingCoin += OnFlippingCoin;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvents.FlippingCoin -= OnFlippingCoin;

            base.UnsubscribeEvents();
        }

        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            Timing.CallDelayed(2f, () =>
            {
                if (ev.IsTails && !ev.Player.IsDead)
                {
                    ev.Player.IsGodModeEnabled = false;
                    ev.Player.Explode();
                    ev.Player.Kill($"{Plugin.Instance?.Config.LoseCauses.RandomItem()}");
                    return;
                }

                ev.Player.ShowHint($"{Plugin.Instance?.Config.WinHints.RandomItem()}");
                
                Effects.ForEach((effect) => { ev.Player.EnableEffect(effect.Type, effect.Intensity, effect.Duration, true); });
            });
        }
    }
}