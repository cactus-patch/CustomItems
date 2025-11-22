using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using InventorySystem.Items.Firearms.Attachments;
using YamlDotNet.Serialization;
using ItemEvents = Exiled.Events.Handlers.Item;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon
    {
        public override uint Id { get; set; } = 2;
        public override string Name { get; set; } = "SR-118";

        public override string Description { get; set; } =
            "A modified E-11 that fires 5.56 at supersonic velocity that deals significantly more damage";

        public override float Weight { get; set; } = 4f;
        public override float Damage { get; set; } = 125f;
        public override byte ClipSize { get; set; } = 1;

        [YamlIgnore]
        public override AttachmentName[] Attachments { get; set; } =
        [
            AttachmentName.LowcapMagAP, AttachmentName.Foregrip, AttachmentName.DotSight,
            AttachmentName.RecoilReducingStock, AttachmentName.CarbineBody, AttachmentName.MuzzleBrake,
        ];

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoomSpawnPoints =
            [
                new RoomSpawnPoint { Room = RoomType.HczArmory, Chance = 100, },
            ],
        };

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
            try
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint("You are not allowed to change the attachment for this weapon.");
            }
            catch (Exception ex) {
                Log.Error($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                ev.Player.CurrentItem.Destroy();
                ev.Player.CurrentItem = null;
                ev.Player.ShowHint("An error has occured and the Sniper has been removed");
            }
        }
    }
}