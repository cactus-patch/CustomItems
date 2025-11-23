using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using ItemEvents = Exiled.Events.Handlers.Item;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.GunRevolver)]
    public class AdminAbuse : CustomWeapon
    {
        [Description("thanks to hayden for the idea on buckshot or else it wouldnt have worked")]
        public override uint Id { get; set; } = 20;

        public override ItemType Type { get; set; } = ItemType.GunRevolver;
        public override string Name { get; set; } = "Regert (dont use this because it crashes the server)";
        public override string Description { get; set; } = "you asked for it!";
        public override float Weight { get; set; } = 1f;
        public override float Damage { get; set; } = 0;

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 0,
        };

        public override AttachmentName[] Attachments { get; set; } =
        [
            AttachmentName.CylinderMag7,
            AttachmentName.ExtendedBarrel,
            AttachmentName.StandardStock,
        ];

        // I personally like having all the OnEvent before the SubscribeEvents but in the future you can do what you like
        protected override void OnShot(ShotEventArgs ev)
        {
            var throwable = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
            // defining ammo by itself will not change the ammo count
            // so you need to call ev.Firearm.MagazineAmmo :)
            
            // ev.Firearm.MagazineAmmo = E.Subtract((ushort)ev.Firearm.MagazineAmmo);
            
            throwable.Projectile.GameObject.AddComponent<CollisionHandler>()
                .Init(ev.Player.GameObject, throwable.Projectile.Base);

            base.OnShot(ev);
        }

        private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            Log.Debug($"Player {ev.Player.Nickname} tried to change attachments for {Name}");
            ev.IsAllowed = false;
            ev.Player.ShowHint("You are not allowed to change the attachment for this weapon.");
            
            base.OnChangingAttachment(ev);
        }

        [Description("can be removed if not necesarry")]
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
    }
}