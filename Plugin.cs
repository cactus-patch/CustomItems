using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace ExtendedItems
{
    public class Plugin : Plugin<Config> 
    {
        public override string Prefix => "Cactus Patch";
        public override string Name => "Custom Items";
        public override string Author => "Noobest1001";
        public override Version Version => new(3, 0, 4);
        public override Version RequiredExiledVersion => new(9, 5, 0);
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
}