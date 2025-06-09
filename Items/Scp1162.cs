using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System.ComponentModel;
using System;
using UnityEngine;
using YamlDotNet.Serialization;
using Random = System.Random;
using ServerEvents = Exiled.Events.Handlers.Server;
using PlayerEvent = Exiled.Events.Handlers.Player;
using E = ExtendedItems.Utils;
using InventorySystem.Items.Usables;
using InventorySystem.Items.Usables.Scp330;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.SCP500)]
    public class Scp1162 : CustomItem
    {
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
            ItemType.KeycardJanitor,
            ItemType.KeycardZoneManager,
            ItemType.KeycardScientist,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardResearchCoordinator,
            ItemType.KeycardMTFPrivate,
            ItemType.KeycardMTFOperative,
            ItemType.KeycardMTFCaptain,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardO5,
            ItemType.SurfaceAccessPass,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.Painkillers,
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.SCP207,
            ItemType.AntiSCP207,
            ItemType.GrenadeHE,
            ItemType.GrenadeFlash,
            ItemType.Coin,
            ItemType.Flashlight,
            ItemType.Radio,
            ItemType.Ammo9x19

        ];

        public Tuple<string, uint> ServerJoin { get; set; } = new("Welcome to the server!", 10);

        private void OnRoundStarted()
        {
            Room room = Room.Get(Plugin.Instance.Config.Scp1162Room);
            Vector3 globalPos = E.GetGlobalCords(Plugin.Instance.Config.Scp1162Room, new Vector3(16.68f, 11.6f, 8.11f));

            Quaternion rotation = room.Rotation;
            Vector3 rot = new(0f, 1f, 0.0f);
            Quaternion quaternion = Quaternion.Euler(rot.x, rotation.eulerAngles.y + rot.y, rot.z);

            Exiled.API.Features.Pickups.Pickup item = Spawn(globalPos)!;

            item.Rotation = quaternion;
            item.Rigidbody.useGravity = false;
            item.Rigidbody.detectCollisions = false;
        }

        protected override void SubscribeEvents()
        {
            ServerEvents.RoundStarted += OnRoundStarted;
            PlayerEvent.PickingUpItem += OnPickingUp;
            PlayerEvent.DroppingItem += OnDroppingItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            PlayerEvent.PickingUpItem -= OnPickingUp;
            PlayerEvent.DroppingItem -= OnDroppingItem;

            base.UnsubscribeEvents();
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.Item.Destroy();

            Room room = Room.Get(Plugin.Instance.Config.Scp1162Room);
            Vector3 globalPos = E.GetGlobalCords(Plugin.Instance.Config.Scp1162Room, new Vector3(16.68f, 11.6f, 8.11f));

            Quaternion rotation = room.Rotation;
            Vector3 rot = new(0f, 1f, 0.0f);
            Quaternion quaternion = Quaternion.Euler(rot.x, rotation.eulerAngles.y + rot.y, rot.z);

            Exiled.API.Features.Pickups.Pickup item = Spawn(globalPos)!;

            item.Rotation = quaternion;
            item.Rigidbody.useGravity = false;
            item.Rigidbody.detectCollisions = false;

            base.OnDroppingItem(ev);
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.NetId < 2) return;
            ev.IsAllowed = false;
            int item = new();
            Random candy = new();
            Item Currentlyitem = ev.Player.CurrentItem;
            ItemType ItemtoGive;
            try
            {
                if(ev.Player.CurrentItem is null || ev.Player.CurrentItem.Type == ItemType.None)
                {
                    ev.Player.ShowHint("You insert your hands into SCP-1162 and lose feeling in them.", 5);
                    ev.Player.ShowHitMarker(2);
                    ev.Player.EnableEffect(EffectType.SeveredHands);
                    return;
                }
                else if(Currentlyitem.Type == ItemType.SCP330)
                {
                    ev.Player.ShowHint("You put an SCP-330-1 instance in and SCP-1162 just spits it back out");
                    return;
                }
                ev.Player.RemoveHeldItem();
                if (_rng.NextDouble() < LoseChance)
                {
                    ev.Player.ShowHint("You insert your item into SCP-1162 and it gets destroyed.", 5);
                    return;
                }
                ItemtoGive = ItemTypes[_rng.Next(0, ItemTypes.Length)];
                if(ItemtoGive == ItemType.Ammo9x19)
                {
                    var thing = candy.Next(0, 6);
                    ItemtoGive = ItemType.None;
                    ev.Player.TryAddCandy(E.AddCandy(thing));
                }

            }
            catch (Exception ex) 
            {
                var timeUtc = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime EstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                string now = EstTime.ToString("dd, hh:mm:ss.ff tt");
                Log.Error($"Error: {ex} occured.");
                ev.Player.ShowHint($"Something went wrong at {now}.\nplease send a DM to Cactusman or Noobest1001 on Discord or make an issue on Github.", 5);  
            }

            base.OnPickingUp(ev);
        }
        

        private void SpawnRoom(RoomType room, Vector3 offset)
        {
            Room temp = Room.Get(room);
            Vector3 globalPos = Utils.GetGlobalCords(room, offset);

            Quaternion rotation = temp.Rotation;
            Vector3 rot = new(0f, 1f, 0.0f);
            Quaternion quaternion = Quaternion.Euler(rot.x, rotation.eulerAngles.y + rot.y, rot.z);

            Exiled.API.Features.Pickups.Pickup item = Spawn(globalPos)!;


            item.Rotation = quaternion;
            item.Rigidbody.useGravity = false;
            item.Rigidbody.detectCollisions = false;
        }
    }
}
