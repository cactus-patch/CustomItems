using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace CustomItems;

public class Plugin : Plugin<Config> {
    public override string Prefix => "Cactus Patch";
    public override string Name => "Custom Items";
    public override string Author => "Noobest1001";
    public override Version Version => new(9, 0, 0);
    public override Version RequiredExiledVersion => new(3, 1, 1);
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
