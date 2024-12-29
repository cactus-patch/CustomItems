using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using Random = UnityEngine.Random;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace CustomItems.Items;

[CustomItem(ItemType.SCP500)]
public class Scp1162 : CustomItem {
  public override string Name { get; set; } = "SCP-1162";
  public override uint Id { get; set; } = 432;
  public override string Description { get; set; } = "Hold an item and pick it up to get another.";
  public override float Weight { get; set; } = 0f;
  public override Vector3 Scale { get; set; } = new(10f, 0.1f, 10f);

  public override SpawnProperties? SpawnProperties { get; set; }

  [Description("Types of items that can be traded from SCP-1162.")]
  public ItemType[] ItemTypes { get; set; } = [
    ItemType.None,
    ItemType.Medkit,
    ItemType.Adrenaline,
    ItemType.KeycardGuard,
    ItemType.KeycardScientist,
    ItemType.KeycardZoneManager
  ];

  private static Vector3 GetGlobalCords(Vector3 localPos, Room room) {
    var rotation = room.Rotation;
    var roomPos = room.Position;
    if (Math.Abs(rotation.eulerAngles.y) < 1.0)
      return new Vector3(roomPos.x + localPos.x, roomPos.y + localPos.y, roomPos.z + localPos.z);
    if (Math.Abs(rotation.eulerAngles.y - 90f) < 1.0)
      return new Vector3(roomPos.x + localPos.z, roomPos.y + localPos.y, roomPos.z - localPos.x);
    if (Math.Abs(rotation.eulerAngles.y - 180f) < 1.0)
      return new Vector3(roomPos.x - localPos.x, roomPos.y + localPos.y, roomPos.z - localPos.z);
    if (Math.Abs(rotation.eulerAngles.y - 270f) < 1.0)
      return new Vector3(roomPos.x - localPos.z, roomPos.y + localPos.y, roomPos.z + localPos.x);
    return Vector3.zero;
  }


  private void OnRoundStarted() {
    var room = Room.Get(RoomType.Lcz173);
    var localPos = new Vector3(16.68f, 11.65f, 8.11f);
    var globalPos = GetGlobalCords(localPos, room);
    var rotation = room.Rotation;
    var rot = new Vector3(0f, 1f, 0.0f);
    var quaternion = Quaternion.Euler(rot.x, rotation.eulerAngles.y + rot.y, rot.z);

    var item = Spawn(globalPos)!;
    item.Rotation = quaternion;
    item.Rigidbody.useGravity = false;
    item.Rigidbody.detectCollisions = false;
  }

  protected override void SubscribeEvents() {
    ServerEvents.RoundStarted += OnRoundStarted;

    base.SubscribeEvents();
  }

  protected override void UnsubscribeEvents() {
    ServerEvents.RoundStarted -= OnRoundStarted;

    base.UnsubscribeEvents();
  }

  protected override void OnPickingUp(PickingUpItemEventArgs ev) {
    ev.IsAllowed = false;
    var item = ev.Player.CurrentItem;
    if (item == null) {
      ev.Player.EnableEffect(EffectType.SeveredHands, byte.MaxValue);
    }
    else {
      ev.Player.RemoveItem(item);
      item = ev.Player.AddItem(ItemTypes[Random.Range(0, ItemTypes.Length)]);
      ev.Player.CurrentItem = item;
    }

    base.OnPickingUp(ev);
  }
}