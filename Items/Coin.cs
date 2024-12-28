using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerEvents = Exiled.Events.Handlers.Player;
using MEC;

namespace CustomItems.Items;

[CustomItem(ItemType.Coin)]
public class Coin : CustomItem {
    public override string Name { get; set; } = "Coin";
    public override uint Id { get; set; } = 393;
    public override string Description { get; set; } = "<i>\"What's the most you ever lost on a coin toss?\"</i>";
    public override float Weight { get; set; } = 1f;
    
    public override SpawnProperties? SpawnProperties { get; set; } = new() {
      Limit = 3,
      DynamicSpawnPoints = [
        new DynamicSpawnPoint { Location = SpawnLocationType.InsideLczCafe, Chance = 50 },
        new DynamicSpawnPoint { Location = SpawnLocationType.InsideLczWc, Chance = 50 },
        new DynamicSpawnPoint { Location = SpawnLocationType.InsideHczArmory, Chance = 50 },
        new DynamicSpawnPoint { Location = SpawnLocationType.InsideEscapeSecondary, Chance = 50 }
      ]
    };

    public record Effect(EffectType Type, float Duration, byte Intensity);

    [Description("Effects to give if coin landed on heads.")]
    public Effect[] Effects { get; set; } = [
      new (EffectType.DamageReduction, 30, 125),
      new (EffectType.RainbowTaste, 30, byte.MaxValue),
      new (EffectType.Invigorated, 30, byte.MaxValue),
      new (EffectType.MovementBoost, 30, 125)
    ];
  
    protected override void SubscribeEvents() {
      PlayerEvents.FlippingCoin += OnFlippingCoin;
      
      base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents() {
      PlayerEvents.FlippingCoin -= OnFlippingCoin;
      
      base.UnsubscribeEvents();
    }

    private void OnFlippingCoin(FlippingCoinEventArgs ev) {
      if (!Check(ev.Item)) return;
      Timing.CallDelayed(2f, () => {
        if (ev.IsTails && !ev.Player.IsDead) {
          ev.Player.IsGodModeEnabled = false;
          ev.Player.Explode();
          ev.Player.Kill("You lost.");
          return;
        }

        ev.Player.ShowHint("You won -- you feel the adrenaline rushing in your veins.");
        Effects.ForEach((effect) => {
          ev.Player.EnableEffect(effect.Type, effect.Intensity, effect.Duration, addDurationIfActive: true);
        });
      });
    }
}