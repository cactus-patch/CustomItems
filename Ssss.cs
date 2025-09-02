using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace ExtendedItems
{
    public class Ssss
    {
        public static IEnumerable<SettingBase> _settings;

        public static void Register()
        {
            Log.Info("Keybind Registered");
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += Keybind;

            _settings =
            [
                new HeaderSetting(10, "Example Header", "Example Header Description", true),
                new KeybindSetting(24, "Example Keybind", KeyCode.Delete, hintDescription: "Example"),
                
            ];
            SettingBase.Register(_settings);
        }

        public static void Unregister()
        {
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= Keybind;
        }
        
        public static void Keybind(ReferenceHub referenceHub, ServerSpecificSettingBase settingBase)
        {
            if (settingBase is not SSKeybindSetting keybindSetting || keybindSetting.SettingId != 24 || !keybindSetting.SyncIsPressed)
                return;
            if (!Player.TryGet(referenceHub, out Player player))
                return;
            
            Log.Info("Keybind used by " + player.Nickname);
            var i = 0;
            foreach (var charge in Items.Plastic.PlacedCharges.ToList())
            {
                float posy = charge.Key.Position.y;
                if (charge.Value != player) continue;

                if (player.Position.y >= posy - 100 && player.Position.y <= posy + 100)
                {
                    Items.Plastic.Instance.Handler(charge.Key, Items.Plastic.C4RemoveMethod.Detonate, player);
                    i++;
                }
                else
                {
                    player.SendConsoleMessage($"One of your charges is out of range. You need to get within the zone that it was placed", "yellow");
                }
                player.ShowHint(i == 1
                    ? $"\n<color=green>{i} C4 charge has been detonated!</color>"
                    : $"\n<color=green>{i} C4 charges have been detonated!</color>");
            }

            // Code Here.
        }
    }
}