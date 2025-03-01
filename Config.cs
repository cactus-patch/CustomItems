using ExtendedItems.Items;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ExtendedItems
{
    public class Config : IConfig 
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Weather the Tranq is effective on Tutorials")]
        public bool EffectiveOnTutorials = false;

        public Coin Coin { get; set; } = new();
        public Scp1162 Scp1162 { get; set; } = new();
        public Scp1499 Scp1499 { get; set; } = new();
        public GrenadeLauncher GrenadeLauncher { get; set; } = new();
        public Sniper Sniper { get; set; } = new();
        public Tranquilizer Tranquilizer { get; set; } = new();
    }
}
