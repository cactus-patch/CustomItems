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
	public override string Description { get; set; } = "Grenade Launcher";
	public override float Weight { get; set; } = 10f;
	public override SpawnProperties? SpawnProperties { get; set; } = new () { RoomSpawnPoints = [ new RoomSpawnPoint() { Room = RoomType.Surface, Chance = 100 } ] };
	public override float Damage { get; set; } = 0f;
	public override byte ClipSize { get; set; } = 1;

	protected override void OnShooting(ShootingEventArgs ev) {
		var throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
		throwable.Projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, throwable.Projectile.Base);
		ev.Player.AddAmmo(AmmoType.Nato762, 1);
		ev.Firearm.MagazineAmmo = 0;
		ev.Firearm.BarrelAmmo = 0;

		base.OnShooting(ev);
	}

	protected override void OnReloading(ReloadingWeaponEventArgs ev) {
		var grenade = ev.Player.Items.First((item) => item.Type == ItemType.GrenadeHE);
		bool hasBoom = ev.Player.Inventory.TryGetInventoryItem(ItemType.GrenadeHE, out ItemBase value);
		if (hasBoom)
		{
			ev.IsAllowed = false;
			ev.Player.ShowHint("You need to find a High Explosive Grenade");
			base.OnReloading(ev);
			return;
		}
		ev.Player.RemoveItem(grenade);
		base.OnReloading(ev);

	base.OnReloading(ev);
	}

	private static IEnumerator<float> Detonate(Throwable throwable) {
	for (;;) {
			float comp = -2;
			var yVelocity = throwable.Projectile.Rigidbody.velocity.y - comp;
			Log.Info(yVelocity);
	}
	}
}
