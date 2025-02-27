using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using YamlDotNet.Serialization;
using Random = System.Random;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp1162 : CustomItem {
        public override string Name { get; set; } = "SCP-1162";
        public override uint Id { get; set; } = 7825;
        public override string Description { get; set; } = "Hold an item and pick it up to get another.";
        public override float Weight { get; set; } = 0f;
        public override Vector3 Scale { get; set; } = new(5f, 5f, 5f);

        [Description("Chance from 0 to 1 that the item will be destroyed.")]
        public float LoseChance { get; set; } = 0.15f;

        public override SpawnProperties? SpawnProperties { get; set; }
        [YamlIgnore] private readonly Random _rng = new();

        [Description("Types of items that can be traded from SCP-1162.")]
        public ItemType[] ItemTypes { get; set; } = 
        [
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.KeycardGuard,
            ItemType.KeycardScientist,
            ItemType.KeycardZoneManager
        ];

        private void OnRoundStarted() 
        {
            var room = Room.Get(RoomType.Lcz173);
            var globalPos = Utils.GetGlobalCords(RoomType.Lcz173, new Vector3(16.68f, 11.6f, 8.11f));
            var rotation = room.Rotation;
            var rot = new Vector3(0f, 1f, 0.0f);
            var quaternion = Quaternion.Euler(rot.x, rotation.eulerAngles.y + rot.y, rot.z);

            var item = Spawn(globalPos)!;
            item.Rotation = quaternion;
            item.Rigidbody.useGravity = false;
            item.Rigidbody.detectCollisions = false;
        }

        protected override void SubscribeEvents() 
        {
            ServerEvents.RoundStarted += OnRoundStarted;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents() 
        {
            ServerEvents.RoundStarted -= OnRoundStarted;

            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev) 
        {
            ev.IsAllowed = false;

            try {
                var item = ev.Player.CurrentItem;
                if (item == null) 
                {
                    ev.Player.EnableEffect(EffectType.SeveredHands, byte.MaxValue);
                    return;
                }
                else {
                    if(item.Type == ItemType.SCP330)
                    {
                        ev.Player.CurrentItem = null;
                        ev.Player.ShowHint("You can't trade SCP-330 with SCP-1162.", 5);
                        return;
                    }
                    ev.Player.RemoveItem(item);
                    if (_rng.NextDouble() < LoseChance) return;
                    item = ev.Player.AddItem(ItemTypes[_rng.Next(0, ItemTypes.Length)]);
                    ev.Player.CurrentItem = item;
                    return;
                }
            }
            catch {
                // ignored
            }

        base.OnPickingUp(ev);
        }
    }
}
