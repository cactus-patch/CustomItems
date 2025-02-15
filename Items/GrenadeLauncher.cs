using System.Numerics;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using PlayerEvent = Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items;
using Exiled.Events.EventArgs.Player;


namespace CustomItems.Items;

[CustomItem(ItemType.GunLogicer)]
public class GrenadeLauncher : CustomWeapon {
    public override uint Id { get; set; } = 805;
    public override string Name { get; set; } = "Grenade Launcher";
    public override string Description { get; set; } = "A modified Chaos Insergency LMG that fires High Explisove Grenades";
    public override float Weight { get; set; } = 10f;
    public override SpawnProperties? SpawnProperties { get; set; } = new () { RoomSpawnPoints = [ new RoomSpawnPoint() { Room = RoomType.HczHid, Chance = 100 } ] };
    public override float Damage { get; set; } = 0f;
    public override byte ClipSize { get; set; } = 1;


    protected override void OnShooting(ShootingEventArgs ev) {
        var throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
        ushort ammo = Utils.Subtrat((ushort)ev.Firearm.MagazineAmmo);
        throwable.Projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, throwable.Projectile.Base);
        ev.Firearm.MagazineAmmo = 0;
        ev.Firearm.BarrelAmmo = 0;
        ev.Player.AddAmmo(AmmoType.Nato762, ammo);

        base.OnShooting(ev);
    }

    protected override void OnReloading(ReloadingWeaponEventArgs ev) {
        if (Utils.TryRemoveItem(ev.Player, ItemType.GrenadeHE))
        {
            ev.IsAllowed = true;
            base.OnReloading(ev);
        }
        else
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint("You need a High Explosive Grenade to reload this weapon" , 5 );
            base.OnReloading(ev);
        }
    }

    private static IEnumerator<float> Detonate(Throwable throwable) {
        for (;;) {
            float comp = -2;
            var yVelocity = throwable.Projectile.Rigidbody.velocity.y - comp;
            Log.Info(yVelocity);
        }
    }
}
