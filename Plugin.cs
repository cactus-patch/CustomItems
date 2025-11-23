using Discord;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace ExtendedItems
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin : Plugin<Config>
    {
        public static Plugin? Instance;

        private List<ServerSpecificSettingBase>? _settings;
        public override string Prefix => "Extended Items";
        public override string Name => "Extended Items";
        public override string Author => "Noobest1001";
        public override Version Version => new(3, 3, 0, 0);
        public override Version RequiredExiledVersion => new(9, 7, 0);

        public override void OnEnabled()
        {
            Instance = this;

            Ssss.Register();

            Log.Send("Registering items", LogLevel.Info, ConsoleColor.DarkYellow);
            CustomItem.RegisterItems(overrideClass: Config);

            _settings =
            [
                new SSGroupHeader(10, "Extended Items"),
                new SSKeybindSetting(24, "C4 Detonation", KeyCode.Delete, hint: "Explodes all C4 placed by you"),
            ];

            ServerSpecificSettingsSync.DefinedSettings = _settings.ToArray();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Send("Unregistering items", LogLevel.Info, ConsoleColor.DarkYellow);
            CustomItem.UnregisterItems();

            Ssss.Unregister();

            _settings = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}