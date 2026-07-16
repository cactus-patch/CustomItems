using Discord;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using UserSettings.ServerSpecific;
using Map = Exiled.Events.Handlers.Map;

namespace ExtendedItems;

// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : Plugin<Config>
{
    public static Plugin? Instance;

    public static EventHandler? _event;

    private List<ServerSpecificSettingBase>? _settings;
    public override string Name => "Extended Items";
    public override string Author => "Noobest1001";
    public override Version Version => new(4, 0, 0, 1);
    public override Version RequiredExiledVersion => new(9, 14, 2);

    public override void OnEnabled()
    {
        Instance = this;
        _event = new EventHandler();

        Ssss.Register();

        Log.Send("Registering items", LogLevel.Info, ConsoleColor.DarkYellow);
        CustomItem.RegisterItems(overrideClass: Config);

        _settings =
        [
            new SSGroupHeader(10, "Cactus Patch"),
            new SSKeybindSetting(24, "C4 Detonation", KeyCode.Delete, hint: "Explodes all C4 placed by you")
        ];

        ServerSpecificSettingsSync.DefinedSettings = _settings.ToArray();

        Map.ExplodingGrenade += EventHandler.OnGrenadeExploding;

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Map.ExplodingGrenade -= EventHandler.OnGrenadeExploding;

        Log.Send("Unregistering items", LogLevel.Info, ConsoleColor.DarkYellow);
        CustomItem.UnregisterItems();

        Ssss.Unregister();

        _settings = null;
        _event = null;
        Instance = null;

        base.OnDisabled();
    }
}