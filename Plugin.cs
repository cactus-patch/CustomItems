using Discord;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using UserSettings.ServerSpecific;

namespace ExtendedItems
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "Cactus Patch";
        public override string Name => "Extended Items";
        public override string Author => "Noobest1001";
        public override Version Version => new(3, 2, 1);
        public override Version RequiredExiledVersion => new(9, 7, 0);
        public static Plugin? Instance;

        private List<ServerSpecificSettingBase>? _settings;

        public override void OnEnabled()
        {
            Instance = this;
            
            Ssss.Register();
            
            Log.Send("Registering items",LogLevel.Info ,ConsoleColor.DarkYellow);
            CustomItem.RegisterItems(overrideClass: Config);
            
            _settings = 
            [
                new SSGroupHeader(10, "Example Header"),
                new SSKeybindSetting(24, "Example Keybind", UnityEngine.KeyCode.Delete, hint: "explodes a set C4"),
            ];

            ServerSpecificSettingsSync.DefinedSettings = _settings.ToArray();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Send("Unregistering items",LogLevel.Info ,ConsoleColor.DarkYellow);
            CustomItem.UnregisterItems();

            Ssss.Unregister();
            
            _settings = null;
            
            Instance = null;

            base.OnDisabled();
        }
    }
}