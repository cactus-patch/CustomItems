using System.ComponentModel;
using ExtendedItems.Types;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using YamlDotNet.Serialization;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Random = System.Random;
using Exiled.API.Extensions;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.Coin)]
    public class Coin : CustomItem {
        public override string Name { get; set; } = "SCP-1289";
        public override uint Id { get; set; } = 804;
        public override string Description { get; set; } = "<i>\"What's the most you ever lost on a coin toss?\"</i>";
        public override float Weight { get; set; } = 1f;
        [YamlIgnore] private readonly Random _rng = new();
        private string[] WinHints { get; set; } = 
        [
            "Long Live the King",
            "Use Force",
            "Utilize Might",
            "Life to the Ruler",
            "Be Brave",
            "The Savior is here",
            "The end is never near"
        ];

        private string[] LoseCauses {get; set;} = 
        [
            "Silence",
            "Quiet",
            "Don't Look",
            "Look Away",
            "Death to the King",
            "Death to the Ruler",
            "The End",
        ];

        public override SpawnProperties? SpawnProperties { get; set; } = new() {
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
            Timing.CallDelayed(2f, () => {
                if (ev.IsTails && !ev.Player.IsDead) {
                    ev.Player.IsGodModeEnabled = false;
                    ev.Player.Explode();
                    ev.Player.Kill($"{LoseCauses.GetRandomValue}");
                    return;
                }

                ev.Player.ShowHint($"{WinHints.GetRandomValue}");
                Effects.ForEach((effect) => { ev.Player.EnableEffect(effect.Type, effect.Intensity, effect.Duration, true); });
            });
        }
    }
}