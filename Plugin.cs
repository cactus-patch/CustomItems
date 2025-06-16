using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace ExtendedItems
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "Cactus Patch";
        public override string Name => "Extended Items";
        public override string Author => "Noobest1001";
        public override Version Version => new(3, 2, 1);
        public override Version RequiredExiledVersion => new(9, 6, 0);
        public static Plugin? Instance;

        public override void OnEnabled()
        {
            Instance = this;

            Log.Info("Registering items");
            CustomItem.RegisterItems(overrideClass: Config);


            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Info("Unregistering items");
            CustomItem.UnregisterItems();

            Instance = null;

            base.OnDisabled();
        }
    }
}