using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using YamlDotNet.Serialization;
using E = ExtendedItems.Utils;
using ItemEvents = Exiled.Events.Handlers.Item;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : CustomWeapon
    {
        public override uint Id { get; set; } = 5;
        public override string Name { get; set; } = "Grenade Launcher";

        public override string Description { get; set; } =
            "A modified Chaos Insurgency LMG that fires High Explosive Grenades";

        public override float Weight { get; set; } = 10f;
        public override float Damage { get; set; } = 0f;
        public override byte ClipSize { get; set; } = 1;

        [YamlIgnore]
        public override AttachmentName[] Attachments { get; set; } =
            [AttachmentName.Laser, AttachmentName.IronSights, AttachmentName.ShortBarrel,];

        public override SpawnProperties? SpawnProperties { get; set; } = new()

        {
            Limit = 1,
            DynamicSpawnPoints =
            [
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideHidChamber,
                },
            ],
        };

        protected override void OnShooting(ShootingEventArgs ev)
        {
            var throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
            var ammo = E.Subtract((ushort)ev.Firearm.MagazineAmmo);

            throwable.Projectile.GameObject.AddComponent<CollisionHandler>()
                .Init(ev.Player.GameObject, throwable.Projectile.Base);

            ev.Firearm.MagazineAmmo = 0;
            ev.Firearm.BarrelAmmo = 0;

            ev.Player.AddAmmo(AmmoType.Nato762, ammo);

            base.OnShooting(ev);
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            if (Plugin.Instance is null) return;
            
            if (ev.Player.GetAmmo(AmmoType.Nato762) == 0)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint("You don't have any 7.62mm ammo to reload the grenade launcher!", 5);
            }
            else if (ev.Player.GetAmmo(AmmoType.Nato762) < Plugin.Instance.Config.GrenadeLauncherAmmoUsage)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint($"You need more than {Plugin.Instance.Config.GrenadeLauncherAmmoUsage} 7.62 to reload the grenade launcher!", 5);
            }
            else
            {
                var ammo = ev.Player.GetAmmo(AmmoType.Nato762);
                ammo -= Plugin.Instance.Config.GrenadeLauncherAmmoUsage;
                ev.Player.SetAmmo(AmmoType.Nato762, ammo) ;
            }
            base.OnReloading(ev);
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
            if (!Check(ev.Item)) return;
            Log.Debug($"Player {ev.Player.Nickname} tried to change attachments for {Name}");
            ev.Player.Broadcast(5, "You can't change the attachments on this weapon");
            ev.IsAllowed = false;
            
            base.OnChangingAttachment(ev);
        }
    }
}