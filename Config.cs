using System.ComponentModel;
using Exiled.API.Interfaces;
using ExtendedItems.Items;

namespace ExtendedItems
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;

        #region Config Settings
        
        [Description("The Distance from Larry a player can be before they can no longer use SCP-1499")]
        public float LarryDistance { get; set; } = 10f;

        [Description("The possable hints shown when you Win")]
        public string[] WinHints { get; set; } =
        [
            "Long Live the King",
            "Use Force",
            "Utilize Might",
            "Life to the Ruler",
            "Be Brave",
            "The Savior is here",
            "The end is never near",
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
        public Scp1499 Scp1499 { get; set; } = new();
        public Plastic Plastic { get; set; } = new();

        #endregion
    }
}