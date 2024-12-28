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
    public override Vector3 Scale { get; set; } = new(7f, 0.1f, 7f);
    
    public override SpawnProperties? SpawnProperties { get; set; }
    
    [Description("Types of items that can be traded from SCP-1162.")]
    public ItemType[] ItemTypes { get; set; } = [
      ItemType.None,
      ItemType.Medkit,
      ItemType.Adrenaline,
      ItemType.KeycardGuard,
      ItemType.KeycardScientist,
      ItemType.KeycardZoneManager,
    ];

    private void OnRoundStarted() {
      var room = Room.Get(RoomType.Lcz173);
      var localPos = new Vector3(16.68f, 12f, 8.11f);
      var globalPos = room.Position + localPos;
      var roomRot = room.Rotation;
      if (Math.Abs(roomRot.eulerAngles.y - 90) < 1) {
        globalPos.x += localPos.z;
        globalPos.z -= localPos.x;
      } else if (Math.Abs(roomRot.eulerAngles.y - 180) < 1) {
        globalPos.x -= localPos.x;
        globalPos.z -= localPos.z;
      } else if (Math.Abs(roomRot.eulerAngles.x - 270) < 1) {
        globalPos.x -= localPos.z;
        globalPos.z += localPos.x;
      }

      var item = Spawn(globalPos)!;
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
        return;
      }
      
      ev.Player.RemoveItem(item);
      item = ev.Player.AddItem(ItemTypes[Random.Range(0, ItemTypes.Length)]);
      ev.Player.CurrentItem = item;
      base.OnPickingUp(ev);
    }
}