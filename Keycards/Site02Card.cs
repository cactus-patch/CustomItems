using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LabApi.Features.Wrappers;

namespace ExtendedItems.Keycards
{
    [CustomItem(ItemType.KeycardCustomTaskForce)]
    public class Site02Card : CustomKeycard
    {
        public override uint Id { get; set; } = 90;
        public override string Name { get; set; } = "Site-02 Keycard";
        public override string Description { get; set; } = "A base keycard that is used to give a person a keycard with the right name and for custom colors via a command";
        public override float Weight { get; set; } = 0.01f;

        public override SpawnProperties? SpawnProperties { get; set; } = new()

        {
            Limit = 0,
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        public void CustomKeycard(Player Player, string Name, string LabelHex, string keycardColor, string PermissionsColor, string labelColor)
        {

        }
    }
}
