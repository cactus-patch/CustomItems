using System.ComponentModel;
using CustomItems.Types;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerEvents = Exiled.Events.Handlers.Player;
using MEC;

namespace CustomItems.Items;

[CustomItem(ItemType.SCP268)]
public class SCP1499 : CustomItem {
  public override string Name { get; set; } = "SCP-1499";
  public override uint Id { get; set; } = 394;
  public override string Description { get; set; } = "<i>\"Experience purgatory with a return ticket"</i>";
  public override float Weight { get; set; } = 1f;

  public override SpawnProperties? SpawnProperties { get; set; } = new() {
    Limit = 1,
    DynamicSpawnPoints = [
      new DynamicSpawnPoint() { Location = SpawnLocationType.InsideLczCafe, Chance = 100 },
      /* This is temp.
         Lain ( or Biggie) can you make it so that it can spawn the glass boxes plz :)
      */
    ]
  };
