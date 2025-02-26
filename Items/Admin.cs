using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using YamlDotNet.Serialization;
using UnityEngine;
using ItemEvents = Exiled.Events.Handlers.Item;
using PlayerRoles;

namespace ExtendedItems.Items
{
    [CustomItem(ItemType.ParticleDisruptor)]
    class Admin : CustomWeapon
    {
        public override string Name { get; set; } = "Admin Funny Gun";
        public override uint Id { get; set; } = 806;
        public override string Description { get; set; } = "You should have this";
        public override float Weight { get; set; } = .01f;
        public override float Damage { get; set; } = 100f;
        public override byte ClipSize { get; set; } = 255;
        public override SpawnProperties? SpawnProperties { get; set; } = null;

        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Tutorial)
            {
                var item = ev.Player.CurrentItem;
                ev.IsAllowed = false;
                ev.Player.CurrentItem = null;
                ev.Player.RemoveItem(item);
                base.OnShooting(ev);
            }
            base.OnShooting(ev);

        }
    }
}
