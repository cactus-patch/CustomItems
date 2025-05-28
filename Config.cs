using Exiled.API.Enums;
using Exiled.API.Interfaces;
using ExtendedItems.Items;
using UnityEngine;
using System.ComponentModel;

namespace ExtendedItems
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        #region Config Settings

        [Description("Where SCP-1162 spawns")]
        public RoomType Scp1162Room { get; set; } = RoomType.Lcz173;

        [Description("The offset from Scp1162Room Origin point")]
        public Vector3 SpawnOffset = new(16.68f, 11.6f, 8.11f);

        [Description("Weather a tranqed target hold the item they had when they were tranqed")]
        public bool ReholdItems = false;

        [Description("Weather the Tranq is effective on Tutorials")]
        public bool EffectiveOnTutorials = false;

        [Description("Permissions required to access Debug Commands")]
        public PlayerPermissions DebugPermissions = PlayerPermissions.ServerConsoleCommands;

        [Description("The possable hints shown when you Win")]
        public string[] WinHints { get; set; } =
        [
            "Long Live the King",
            "Use Force",
            "Utilize Might",
            "Life to the Ruler",
            "Be Brave",
            "The Savior is here",
            "The end is never near"
        ];

        [Description("List of possible causes of death when the coin lands on tails")]
        public string[] LoseCauses { get; set; } =
        [
            "Silence",
            "Quiet",
            "Don't Look",
            "Look Away",
            "Death to the King",
            "Death to the Ruler",
            "The End",
        ];

        #endregion
        #region Custom Items setup

        public Coin Coin { get; set; } = new();
        public Scp1162 Scp1162 { get; set; } = new();
        public Scp1499 Scp1499 { get; set; } = new();
        public GrenadeLauncher GrenadeLauncher { get; set; } = new();
        public Sniper Sniper { get; set; } = new();
        public Tranquilizer Tranquilizer { get; set; } = new();
        public Plastic Plastic { get; set; } = new();
        public AdminAbuse AdminAbuse { get; set; } = new();


        #endregion
    }

}
