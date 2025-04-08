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


namespace ExtendedItems.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class Plastic : CustomGrenade
    {
        public override string Name { get; set; } = "Hexahydro-1,3,5-trinitro-1,3,5-triazine";
        public override string Description { get; set; } = "A Remote Explosive that can be detonated when you are within 100m (11.2359550562 Big macs)";
        public override uint Id { get; set; } = 806;
        public override float Weight { get; set; } = 1.5f;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 999f;
        public static Dictionary<Pickup, Player> PlacedCharges { get; } = [];
        public static Plastic Instance { get; private set; } = null!;
        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            RoomSpawnPoints =
            [
                new RoomSpawnPoint() { Room = RoomType.HczNuke, Chance = 50 },
            ],
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideSurfaceNuke,
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

        public void Handler(Pickup? charge, C4RemoveMethod method = C4RemoveMethod.Drop)
        {
            if (charge?.Position is null) return;
            
            switch (method)
            {
                case C4RemoveMethod.Detonate:
                { 
                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                    grenade.FuseTime = 0.1f;
                    
                    grenade.SpawnActive(charge.Position); 
                    break;
                }

                case C4RemoveMethod.Drop:
                {
                    TrySpawn(Id, charge.Position, out _);
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
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Shooting -= OnShooting;

            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            PlacedCharges.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if(!PlacedCharges.ContainsKey(ev.Projectile)) PlacedCharges.Add(ev.Pickup, ev.Player);
            
            base.OnThrownProjectile(ev);
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            Vector3 pos = ev.Position;

            foreach (Player pl in Player.List)
            {
                if (Vector3.Distance(pl.Position, pos) > 100) return;
                if (pl.Role.Team != ev.Player.Role.Team && !pl.IsCuffed) return;
                
                float maxArtificialHealth = 300;
                float currentArtificialHealth = pl.ArtificialHealth;
                float newArtificialHealth = currentArtificialHealth + maxArtificialHealth;
                
                pl.ArtificialHealth = newArtificialHealth;
                
                Timing.CallDelayed(0.1f, () =>
                {
                    if (pl.ArtificialHealth > maxArtificialHealth)
                    {
                        pl.ArtificialHealth = maxArtificialHealth;
                    }
                });
            }
            PlacedCharges.Remove(Pickup.Get(ev.Projectile.Base));
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player) Handler(charge.Key, Plastic.C4RemoveMethod.Remove);
            }
        }

        void OnDied(DiedEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player)
                {
                    Handler(charge.Key, C4RemoveMethod.Drop);
                }
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            Vector3 forward = ev.Player.CameraTransform.forward;

            if (!Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500)) 
                return;
            
            EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
            
            if (grenade == null) return;
            if (PlacedCharges.ContainsKey(Pickup.Get(grenade))) 
            {
                Handler(Pickup.Get(grenade), Plastic.C4RemoveMethod.Remove);
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PlacedCharges.Clear();
        }
    }
}