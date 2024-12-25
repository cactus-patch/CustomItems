using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomModules.API.Features.Attributes;
using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomItems.Items;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomItems.Items;

public class SCP1162 {
  private static readonly ItemType[] ItemTypes = [
    ItemType.None,
    ItemType.Medkit,
    ItemType.Adrenaline,
    ItemType.KeycardGuard,
    ItemType.KeycardScientist,
    ItemType.KeycardZoneManager
  ];

  [ModuleIdentifier]
  public class Item : CustomItem<Behaviour> {
    public override string Name => "SCP-1162";
    public override uint Id => 432;
    public override string Description => "Hold an item and pick it up to get another.";
    public override ItemType ItemType => ItemType.SCP500;

    public override SettingsBase Settings => new Settings {
      Scale = Vector3.one * 3,
      SpawnProperties = new SpawnProperties {
        StaticSpawnPoints = [
          new StaticSpawnPoint { Position = Room.Get(RoomType.Lcz173).Position + Vector3.up * 1.5f }
        ]
      }
    };
  }

  public class Behaviour : ItemBehaviour {
    protected override void OnPickingUp(PickingUpItemEventArgs ev) {
      var item = ev.Player.CurrentItem;
      if (item == null) {
        ev.Player.EnableEffect(EffectType.SeveredHands, byte.MaxValue);
        return;
      }

      ev.Player.RemoveItem(item);
      ev.Player.AddItem(ItemTypes[Random.Range(0, ItemTypes.Length)]);
      // base.OnPickingUp(ev);
    }
  }
}