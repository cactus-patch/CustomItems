using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;
using System.Runtime.InteropServices;
using System.Linq;
using Exiled.API.Features.Roles;
using PlayerRoles;


namespace ExtendedItems.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class Plastic : CustomGrenade
    {
        // Former Name: Hexahydro-1,3,5-trinitro-1,3,5-triazine
        public override string Name { get; set; } = "C4 Explosive Charge";
        public override string Description { get; set; } = "A Remote Explosive that can be detonated when you are within 100m (about 1049.866 Big macs)";
        public override uint Id { get; set; } = 806;
        public override float Weight { get; set; } = 1.5f;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 999f;
        public static Dictionary<Pickup, Player> PlacedCharges { get; } = [];
        public static Dictionary<ushort, Player> Charges { get; } = [];
        public static Plastic Instance { get; private set; } = null!;
        public override SpawnProperties? SpawnProperties { get; set; } = new()

        {
            Limit = 2,
            RoomSpawnPoints =
            [
                new RoomSpawnPoint() { Room = RoomType.HczNuke, Chance = 50 },
            ],
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideEscapePrimary,
                },
            ],
        };

        public enum C4RemoveMethod
        {
            Remove = 0,
            Detonate = 1,
            Drop = 2,
        }


        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        public void Handler(Pickup? charge, C4RemoveMethod method = C4RemoveMethod.Drop, Player? detonator = null)
        {
            if (charge?.Position is null) return;

            if (method == C4RemoveMethod.Detonate && detonator == null && charge != null) { method = C4RemoveMethod.Drop; }
            else { detonator = Charges.TryGetValue(charge.Serial, out var foundPlayer) ? foundPlayer : null; }
                

            switch (method)
            {
                case C4RemoveMethod.Detonate:
                    {
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type, detonator);
                        grenade.FuseTime = 0.1f;
                        grenade.SpawnActive(charge.Position);
                        break;
                    }
                case C4RemoveMethod.Drop:
                    {
                        TrySpawn(Id, charge.Position, out _);
                        break;
                    }
                case C4RemoveMethod.Remove:
                    {
                        charge.Destroy();
                        break;
                    }
            }

            PlacedCharges.Remove(charge);
            charge.Destroy();
        }

        protected override void SubscribeEvents()
        {
            Instance = this;

            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Shooting += OnShooting;

            ServerEvent.RoundEnded += OnRoundEnded;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Shooting -= OnShooting;
            ServerEvent.RoundEnded -= OnRoundEnded;

            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            PlacedCharges.Clear();
            Server.FriendlyFire = false;
            base.OnWaitingForPlayers();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!PlacedCharges.ContainsKey(ev.Projectile))
            {
                PlacedCharges.Add(ev.Pickup, ev.Player);
                Charges.Add(ev.Projectile.Serial, ev.Player);
            }


            base.OnThrownProjectile(ev);
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            // There is a more efficient way to do this, but I could not give two shits.
            // If you find a better solution AND IT WORKS, you can add it :)
            Vector3 Origin = ev.Projectile.Position;
            ushort sn = ev.Projectile.Serial;
            Player detonator = Plastic.Charges[sn];
            Player[] affected = [.. ev.TargetsToAffect.Where(x => Utils.GlobalDet == null || x.Role.Team != Utils.GlobalDet.Role.Team)];
            ev.TargetsToAffect.Clear();

            foreach (Player player in affected)
            {
                    ev.TargetsToAffect.Add(player);
            }

            var keysToRemove = PlacedCharges.Where(k => k.Value == detonator).Select(k => k.Key)
                .ToList();

            foreach (var player in keysToRemove)
            {
                Plastic.PlacedCharges.Remove(player);
            }
            Charges.Remove(sn);

            PlacedCharges.Remove(Pickup.Get(ev.Projectile.Base));
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player) Handler(charge.Key, C4RemoveMethod.Remove);
            }
        }

        void OnDied(DiedEventArgs ev)
        {
            foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player)
                {
                    Handler(charge.Key,C4RemoveMethod.Drop);
                }
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            Vector3 forward = ev.Player.CameraTransform.forward;

            if (!Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out RaycastHit hit, 500))
                return;

            EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();

            if (grenade == null) return;
            if (PlacedCharges.ContainsKey(Pickup.Get(grenade)))
            {
                Handler(Pickup.Get(grenade), C4RemoveMethod.Remove);
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PlacedCharges.Clear();
        }
    }
}
