// Exiled imports

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;
using Map = Exiled.Events.Handlers.Map;

// shortened imports
using PlayerEvent = Exiled.Events.Handlers.Player;
using RoundEndedEventArgs = Exiled.Events.EventArgs.Server.RoundEndedEventArgs;
using ServerEvent = Exiled.Events.Handlers.Server;

namespace ExtendedItems.Items;

[CustomItem(ItemType.GrenadeHE)]
public class Plastic : CustomGrenade
{
    public enum C4RemoveMethod
    {
        Remove = 0,
        Detonate = 1,
        Drop = 2
    }

    // Former Name: Hexahydro-1,3,5-trinitro-1,3,5-triazine
    public override string Name { get; set; } = "C4 Explosive Charge";

    public override string Description { get; set; } =
        "A Remote Explosive that can be detonated when you are within 100m (about 1049.866 Big macs)";

    public override uint Id { get; set; } = 6;
    public override float Weight { get; set; } = 1.5f;
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 0
    };
    public override bool ExplodeOnCollision { get; set; } = false;
    public override float FuseTime { get; set; } = 10800f;
    public static Dictionary<Pickup, Player> PlacedCharges { get; } = [];
    private static Dictionary<ushort, Player> Charges { get; } = [];

    // ReSharper disable once RedundantDefaultMemberInitializer
    public static Plastic? Instance { get; private set; } = null!;

    private Vector3 _toSpawn;
    
    public readonly int Limit = 1;

    public SpawnProperties? SpawnPoints { get; set; } = new()
    {
        Limit = 0,
        LockerSpawnPoints =
        {
            new LockerSpawnPoint
            {
                Type = LockerType.Scp500Pedestal,
                Chance = 70
            },
            new LockerSpawnPoint
            {
                Type = LockerType.AntiScp207Pedestal,
                Chance = 1
            },
            new LockerSpawnPoint
            {
                Type = LockerType.LargeGun,
                Chance = 39
            }
        }
    };

    public override ItemType Type { get; set; } = ItemType.GrenadeHE;

    private readonly Dictionary<Vector3, float> _c4Spawns = Utils.GetSpawnLocations(Instance?.SpawnPoints);

    private void OnFillingLocker(FillingLockerEventArgs ev)
    {
        if (ev.Chamber is null || ev.Pickup is null)
        {
            return;
        }

        Utils.NormalizeLockerSpawns(_c4Spawns, out _toSpawn);
        if (ev.Locker.Position == _toSpawn)
        {
            ev.Chamber.RequiredPermissions = KeycardPermissions.ArmoryLevelThree |
                                             KeycardPermissions.ContainmentLevelTwo | KeycardPermissions.ExitGates;
            ev.IsAllowed = false;
            if (TryGet(6, out var item)) item?.Spawn(ev.Locker.Position);
        }
    }

    public void Handler(Pickup? charge, C4RemoveMethod method = C4RemoveMethod.Drop, Player? detonator = null)
    {
        if (charge is null) return;

        if (detonator == null) method = C4RemoveMethod.Drop;
        else
            detonator = Charges.TryGetValue(charge.Serial, out var foundPlayer)
                ? foundPlayer
                : null;

#pragma warning disable CS8602
        switch (method)
        {
            case C4RemoveMethod.Detonate:
            {
                var grenade = (ExplosiveGrenade)Item.Create(Type);
                grenade.FuseTime = 1f;
                grenade.SpawnActive(charge.Position, detonator);
                break;
            }
            case C4RemoveMethod.Remove:
            {
                charge.Destroy();
                break;
            }
            case C4RemoveMethod.Drop: //This looks ugly af... what the fuck Microsoft
            default:
            {
                TrySpawn(Id, charge.Position, out _);
                break;
            }
        }

        PlacedCharges.Remove(charge);
        charge.Destroy();
    }
#pragma warning restore CS8602
    protected override void SubscribeEvents()
    {
        Instance = this;

        Map.FillingLocker += OnFillingLocker;

        PlayerEvent.Destroying += OnDestroying;
        PlayerEvent.Died += OnDied;
        PlayerEvent.Shooting += OnShooting;

        ServerEvent.RoundEnded += OnRoundEnded;

        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        Map.FillingLocker -= OnFillingLocker;
        
        PlayerEvent.Destroying -= OnDestroying;
        PlayerEvent.Died -= OnDied;
        PlayerEvent.Shooting -= OnShooting;

        ServerEvent.RoundEnded -= OnRoundEnded;

        Instance = null;
        
        base.UnsubscribeEvents();
    }

    protected override void OnWaitingForPlayers()
    {
        PlacedCharges.Clear();
        base.OnWaitingForPlayers();
    }

    protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        Log.Debug("Executing OnThrownProjectile method.");
        if (!PlacedCharges.ContainsKey(ev.Projectile))
        {
            PlacedCharges.Add(ev.Pickup, ev.Player);
            Charges.Add(ev.Projectile.Serial, ev.Player);
        }

        base.OnThrownProjectile(ev);
    }

    protected override void OnExploding(ExplodingGrenadeEventArgs ev)
    {
        Log.Debug("Executing OnExploding method.");
        PlacedCharges.Remove(ev.Projectile);

        base.OnExploding(ev);
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        Log.Debug("Executing OnDestroying method.");
        foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList()
                     .Where(charge => charge.Value == ev.Player))
        {
            Handler(charge.Key, C4RemoveMethod.Remove);
        }
    }

    private void OnDied(DiedEventArgs ev)
    {
        foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList()
                     .Where(charge => charge.Value == ev.Player))
        {
            Handler(charge.Key);
        }
    }

    private void OnShooting(ShootingEventArgs ev)
    {
        var forward = ev.Player.CameraTransform.forward;

        if (!Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
        {
            return;
        }

        var grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();

        if (grenade is null) return;
        if (PlacedCharges.ContainsKey(Pickup.Get(grenade))) Handler(Pickup.Get(grenade), C4RemoveMethod.Remove);
    }

    private static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        PlacedCharges.Clear();
    }
}