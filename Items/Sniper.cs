using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomModules.API.Features.Attributes;
using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomItems.Items.Firearms;

namespace CustomItems.Items;

public class Sniper {
  [ModuleIdentifier]
  public class Item : CustomItem<FirearmBehaviour> {
    public override string Name => "SR-119";
    public override uint Id { get; set; } = 1291;
    public override bool IsEnabled { get; set; } = true;

    public override string Description { get; set; } =
      "A modified E-11 that fires 5.56 at supersonic velocity that deals significantly more damage";

    public override ItemType ItemType => ItemType.GunE11SR;

    public override SettingsBase Settings => new FirearmSettings {
      PickedUpText = new TextDisplay($"You picked up <i>{Name}</i>,<br><i>{Description}</i>",
        channel: TextChannelType.Hint),
      SelectedText = new TextDisplay($"You have selected a <i>{Name}</i>.<br><i>{Description}</i>",
        channel: TextChannelType.Hint),
      ClipSize = 1,
      Damage = 7.5f,
      SpawnProperties = new SpawnProperties() {
        Limit = 1,
        DynamicSpawnPoints = [
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideHczArmory, Chance = 1f }
        ]
      },
      Weight = 8f
    };
  }
}