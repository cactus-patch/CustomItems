using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using PlayerEvent = Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;
using YamlDotNet.Serialization;

namespace CustomItems.Items
{

    [CustomItem(ItemType.GrenadeHE)]
    public class Plastic : CustomItem
    {
        [Description("What happens when the player who threw the C4 dies")]
        public enum PlasticRemovalType
        {
            Remove = 0,
            Explode = 1,
            Drop = 2,
        }
        [Description("Basic stuff DO NOT CHANGED UNLESS DIRECTED TO")]
        public static Plastic Instance { get; private set; } = null!;
        public override string Name { get; set; } = "C4";
        public override uint Id { get; set; } = 806;
        public override string Description { get; set; } = "A remote explosive that can be... well I think you get it";
        public override float Weight { get; set; } = 1f;
        [YamlIgnore] public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        [Description("You can change this stuff tho")]
        public float FuseTime { get; set; } = 9999f;
        [Description("Should the Grenade do something when shot")]
        public bool AllowedShot { get; set; } = true;

        [Description("What happens with C4 charges after they are shot. (Remove / Detonate)")]
        public PlasticRemovalType ShotMethod { get; set; } = PlasticRemovalType.Remove;
        [Description("What happens with C4 charges after the player who threw them dies. (Remove / Detonate / Drop)")]
        public PlasticRemovalType DeathMethod { get; set; } = PlasticRemovalType.Drop;
        public float MaxDistance { get; set; } = 100f;

        public static Dictionary<Pickup, Player> PlacedCharges { get; } = [];
        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoomSpawnPoints =
                [
                    new RoomSpawnPoint() { Room = RoomType.HczArmory, Chance = 100 }
                ]
        };

        [YamlIgnore] public bool ExplodeOnCollision { get; set; } = false;


        public void C4Handler(Pickup? charge, PlasticRemovalType type)
        {
            if (charge?.Position is null) return;
            switch (type)
            {
                case (PlasticRemovalType.Remove):
                    {
                        break;
                    }
                case (PlasticRemovalType.Explode):
                    {
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                        grenade.FuseTime = .1f;
                        grenade.SpawnActive(charge.Position);
                        break;
                    }
                case (PlasticRemovalType.Drop):
                    {
                        TrySpawn(Id, charge.Position, out _);
                        break;
                    }
            }
            PlacedCharges.Remove(charge);
            charge.Destroy();
        }

    }
}
