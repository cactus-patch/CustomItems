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
        ev.Player.EnableEffect(EffectType.DamageReduction, intensity: 125, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.RainbowTaste, intensity: 3, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.MovementBoost, intensity: 3, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.Invigorated, addDurationIfActive: true);
      });
    }
}