using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;

namespace CustomItems.Items;

[CustomItem(ItemType.GunE11SR)]
public class Sniper : CustomWeapon {
  public override uint Id { get; set; } = 1291;
  public override string Name { get; set; } = "SR-119";

  public override string Description { get; set; } =
    "A modified E-11 that fires 5.56 at supersonic velocity that deals significantly more damage";

  public override float Weight { get; set; } = 8f;
  public override float Damage { get; set; } = 118f;
  public override byte ClipSize { get; set; } = 1;

  public override SpawnProperties? SpawnProperties { get; set; } = new() {
    Limit = 1,
    DynamicSpawnPoints = [
      new DynamicSpawnPoint() { Location = SpawnLocationType.InsideHczArmory, Chance = 100 }
    ]
  };

  private void OnChangingAttachment(ChangingAttachmentEventArgs ev)
    {
        if (ev.Attachment.Name != "Low Cap AP Magazine" || ev.Attachment.Name = "Low Cap FMJ magazine")
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint($"You are not allowed to use the {ev.Attachment.Name} attachment.", 5);
        }
    }
}
