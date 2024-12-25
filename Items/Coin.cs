using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomModules.API.Features.Attributes;
using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomItems.Items;
using Exiled.Events.EventArgs.Player;
using PlayerEvents = Exiled.Events.Handlers.Player;
using MEC;

namespace CustomItems.Items;

public class Coin {
  [ModuleIdentifier]
  public class Item : CustomItem<Behaviour> {
    public override string Name => "Coin";
    public override uint Id { get; set; } = 393;
    public override string Description => "\"What's the most you ever lost on a coin toss?\"";

    public override SettingsBase Settings => new Settings {
      PickedUpText = new TextDisplay($"You have picked up a <i>{Name}</i>.<br><i>{Description}</i>",
        channel: TextChannelType.Hint),
      SelectedText = new TextDisplay($"You have selected a <i>{Name}</i>.<br><i>{Description}</i>",
        channel: TextChannelType.Hint),
      NotifyItemToSpectators = true,
      SpawnProperties = new SpawnProperties {
        Limit = 3,
        DynamicSpawnPoints = [
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideLczCafe, Chance = 0.5f },
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideLocker, Chance = 0.3f },
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideServersBottom, Chance = 0.3f },
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideEscapeSecondary, Chance = 0.5f }
        ]
      }
    };
  }

  public class Behaviour : ItemBehaviour {
    protected override void SubscribeEvents() {
      PlayerEvents.FlippingCoin += OnFlippingCoin;

      base.SubscribeEvents();
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
        ev.Player.EnableEffect(EffectType.DamageReduction, 125, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.RainbowTaste, 3, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.MovementBoost, 3, addDurationIfActive: true);
        ev.Player.EnableEffect(EffectType.Invigorated, addDurationIfActive: true);
      });
    }
  }
}