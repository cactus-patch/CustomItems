using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace CustomItems;

public class Plugin : Plugin<Config> {
  public override string Prefix => "[CactusPatch]";
  public override string Name => "CustomItems";
  public override string Author => "furry";
  public override Version Version => new(9, 0, 0);
  public override Version RequiredExiledVersion => new(9, 0, 0);
  public static Plugin? Instance;

  public override void OnEnabled() {
    Instance = this;
    CustomItem.RegisterItems();
    base.OnEnabled();
  }
  
  public override void OnDisabled() {
    Instance = null;
    CustomItem.UnregisterItems();
    base.OnDisabled();
  }
}