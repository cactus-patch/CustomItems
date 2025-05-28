using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

using ItemEvents = Exiled.Events.Handlers.Item;
using E = ExtendedItems.Utils;


namespace ExtendedItems.Items
{

    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : CustomWeapon
    {
        public override uint Id { get; set; } = 805;
        public override string Name { get; set; } = "Grenade Launcher";
        public override string Description { get; set; } = "A modified Chaos Insergency LMG that fires High Explisove Grenades";
        public override float Weight { get; set; } = 10f;

        public override float Damage { get; set; } = 0f;
        public override byte ClipSize { get; set; } = 1;
        public override AttachmentName[] Attachments { get; set; } = [AttachmentName.Laser, AttachmentName.IronSights, AttachmentName.ShortBarrel];
        public override SpawnProperties? SpawnProperties { get; set; } = new()

        {
            Limit = 1,
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideHidChamber,
                },
             ],
        };

        protected override void OnShooting(ShootingEventArgs ev)
        {
            var throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
            ushort ammo = E.Subtract((ushort)ev.Firearm.MagazineAmmo);
            
            throwable.Projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, throwable.Projectile.Base);
            
            ev.Firearm.MagazineAmmo = 0;
            ev.Firearm.BarrelAmmo = 0;
            
            ev.Player.AddAmmo(AmmoType.Nato762, ammo);

            base.OnShooting(ev);
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (ev.Player.CountItem(ItemType.GrenadeHE) > 0)
            {
                ev.IsAllowed = true;
                ev.Player.RemoveItem(ev.Player.Items.First(it => it.Type == ItemType.GrenadeHE));
                base.OnReloading(ev);
            }
            
            else
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(
                    ev.Player.GetAmmo(AmmoType.Nato762) == 0
                        ? "You need 1 7.62 to fire."
                        : "You need a HE Grenade to reload this weapon", 5);
                base.OnReloading(ev);
            }
        }

        protected override void SubscribeEvents()
        {
            ItemEvents.ChangingAttachments += OnChangingAttachments;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            ItemEvents.ChangingAttachments -= OnChangingAttachments;

            base.UnsubscribeEvents();
        }

        private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if(!Check(ev.Item)) { ev.IsAllowed = true; return; }
            
            ev.Player.Broadcast(5, "You can't change the attachments on this weapon");
            ev.IsAllowed = false;
        }

        private static IEnumerator<float> Detonate(Throwable throwable)
        {
            for (; ; )
            {
                float comp = -2;
                var yVelocity = throwable.Projectile.Rigidbody.linearVelocity.y - comp;
                
                Log.Info(yVelocity);
            }
        }
    }
}
