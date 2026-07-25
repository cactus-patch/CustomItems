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
using LabApi.Events.Arguments.ServerEvents;
using UnityEngine;
// shortcuted imports
using PlayerEvent = Exiled.Events.Handlers.Player;
using Random = System.Random;
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
    public override bool ExplodeOnCollision { get; set; } = false;
    public override float FuseTime { get; set; } = 10800f;
    public static Dictionary<Pickup, Player> PlacedCharges { get; } = [];
    private static Dictionary<ushort, Player> Charges { get; } = [];
    public static Plastic Instance { get; private set; } = null!;

    public readonly int Limit = 1;
    
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 0,
    };

    public override ItemType Type { get; set; } = ItemType.GrenadeHE;

    private readonly Dictionary<LockerType, int> Fallback = new() { {LockerType.Scp500Pedestal, 70}, {LockerType.AntiScp207Pedestal, 1}, {LockerType.LargeGun, 39}};

    protected int[] NormalizeC4()
    {
        var NonNormalized = Plugin.Instance.Config.C4Spawns.Values.ToArray();
        var Max = NonNormalized.Sum();
        int[] Normalized = new int[NonNormalized.Length];
        int x = 0;
        
        foreach (var chance in NonNormalized)
        {
            Normalized[x] =  chance / Max;
        }
        Random rand = new();
        double Roll = rand.NextDouble() * Max;
        
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
            case C4RemoveMethod.Drop: //This looks ugly af...  what the fuck Microsoft
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

        PlayerEvent.Destroying += OnDestroying;
        PlayerEvent.Died += OnDied;
        PlayerEvent.Shooting += OnShooting;

        ServerEvent.RoundEnded += OnRoundEnded;

        base.SubscribeEvents();
    }

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
        base.OnWaitingForPlayers();
    }

    protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
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
        Log.Debug("Executing OnDestoying method.");
        foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList()
                     .Where(charge => charge.Value == ev.Player))
            Handler(charge.Key, C4RemoveMethod.Remove);
    }

    private void OnDied(DiedEventArgs ev)
    {
        foreach (KeyValuePair<Pickup, Player> charge in PlacedCharges.ToList()
                     .Where(charge => charge.Value == ev.Player))
            Handler(charge.Key);
    }

    private void OnShooting(ShootingEventArgs ev)
    {
        var forward = ev.Player.CameraTransform.forward;

        if (!Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
            return;

        var grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();

        if (grenade == null) return;
        if (PlacedCharges.ContainsKey(Pickup.Get(grenade))) Handler(Pickup.Get(grenade), C4RemoveMethod.Remove);
    }

    private static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        PlacedCharges.Clear();
    }
}