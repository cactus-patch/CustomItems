using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using ExtendedItems.Items;
using InventorySystem.Items.Firearms.Attachments;

namespace ExtendedItems;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = true;
    public bool EasterEggs { get; set; } = false;

    #region Config Settings
    
    // [Description("How Common Easter Eggs are")]
    // public float EasterEggsChance { get; set; } = 0.01f;

    [Description("Weather the Tranq is effective on Tutorials")]
    public bool EffectiveOnTutorials { get; set; } = false;

    [Description("Weather the Tranq is still useable when Nuke is on")]
    public AttachmentName[] ForcedSniperAttch { get; set; } =
        [AttachmentName.MuzzleBrake, AttachmentName.RecoilReducingStock, AttachmentName.RifleBody];

    [Description("The Places and Chance a C4 can Spawn")]
    public Dictionary<LockerType, int> C4Spawns { get; set; } = new() { {LockerType.Scp500Pedestal, 70}, {LockerType.AntiScp207Pedestal, 1}, {LockerType.LargeGun, 39}};
    
    [Description("The Distance from Larry a player can be before they can no longer use SCP-1499")]
    public float LarryDistance { get; set; } = 10f;

    [Description("The color that glows from where SCP-1289 is")]
    public string SCP1289Color { get; set; } = "White";

    [Description("How long it takes for the glow effect to start around SCP-1289 (in seconds)")]
    public float SCP1289Timer { get; set; } = 10f;

    [Description("How far should the Hint light for SCP-1289 should reach")]
    public float SCP1289LightRange { get; set; } = 10f;

    [Description("How long it take for the glow effect to reach full intensity (in seconds)")]
    public float TimeToFullGlow { get; set; } = 60f;

    [Description("Wether a player that is shot in the head with Sniper when they have heavy armor will have 207?")]
    public bool HeadRemove207 { get; set; } = true;
    
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
        "The End"
    ];

    #endregion

    #region Custom Items setup

    public Coin Coin { get; set; } = new();
    public Scp1499 Scp1499 { get; set; } = new();
    // public GLShot GLShot { get; set; } = new();
    // public GrenadeLauncher GrenadeLauncher { get; set; } = new();
    public Sniper Sniper { get; set; } = new();
    public Tranquilizer Tranquilizer { get; set; } = new();
    public Plastic Plastic { get; set; } = new();
    

    #endregion
}