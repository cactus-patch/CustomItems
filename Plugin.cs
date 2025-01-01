using CustomItems.Items;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace CustomItems;

public class Plugin : Plugin<Config> {
  public override string Name => "CactusPatchCustomItems";
  public override string Author => "furry";
  public override Version Version => new(9, 0, 0);
  public override Version RequiredExiledVersion => new(9, 0, 0);
  public static Plugin? Instance;

  public override void OnEnabled() {
    Instance = this;
    Log.Info("Registering items");
    CustomItem.RegisterItems(overrideClass: Config);
    base.OnEnabled();
  }

  public override void OnDisabled() {
    Instance = null;
    Log.Info("Unregistering items");
    CustomItem.UnregisterItems();
    base.OnDisabled();
  }
}