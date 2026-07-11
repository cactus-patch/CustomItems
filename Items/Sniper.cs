using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
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
            LockerSpawnPoints = 
            [
                new LockerSpawnPoint {Type = LockerType.RifleRack, Chance = 100, UseChamber = true, Zone = ZoneType.HeavyContainment},
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

            // ReSharper disable once InconsistentNaming
            AttachmentName[] Monica = Plugin.Instance!.Config.ForcedSniperAttch;
            AttachmentName[] subMonica = ev.NewAttachmentIdentifiers.Select(user => user.Name).ToArray();
            ev.IsAllowed = subMonica.All(item => Monica.Contains(item));
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (Check(ev.Item))
            {
                ev.IsAllowed = true;
            }
        }
    }
}