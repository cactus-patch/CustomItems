using System.Numerics;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items;


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
        ushort ammo = Sub((ushort)ev.Firearm.MagazineAmmo);
        throwable.Projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, throwable.Projectile.Base);
        ev.Firearm.MagazineAmmo = 0;
        ev.Firearm.BarrelAmmo = 0;
        ev.Player.AddAmmo(AmmoType.Nato762, ammo);

        base.OnShooting(ev);
    }

    protected override void OnReloading(ReloadingWeaponEventArgs ev) {
        var grenade = ev.Player.Items.First((item) => item.Type == ItemType.GrenadeHE);
        if (!ev.Player.Inventory.TryGetInventoryItem(ItemType.GrenadeHE, out ItemBase val))
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint("You need to find a High Explosive Grenade");
            base.OnReloading(ev);
            return;
        }
        ev.Player.RemoveItem(grenade);

        if (Plugin.Instance?.Config?.Debug == true)
        {
            Log.Info(ev.Player.Inventory.TryGetInventoryItem(ItemType.GrenadeHE, out ItemBase value));
            Log.Info("Reloading the Grenade Launcher");
        }

        base.OnReloading(ev);

    base.OnReloading(ev);
    }

    private ushort Sub(ushort inp)
    {
        int temp = inp;
        int m = 1;

        while (!((temp & m) > 0))
        {
            temp = temp ^ m;
            m <<= 1;
        }

        temp = temp ^ m;
        return (ushort) temp;
    }

    private static IEnumerator<float> Detonate(Throwable throwable) {
        for (;;) {
            float comp = -2;
            var yVelocity = throwable.Projectile.Rigidbody.velocity.y - comp;
            Log.Info(yVelocity);
        }
    }
}
