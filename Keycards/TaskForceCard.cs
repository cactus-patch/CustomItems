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
    public class TaskForceCard : CustomKeycard
    {
        public override uint Id { get; set; } = 91;
        public override string Name { get; set; } = "TaskForce Keycard";
        public override string Description { get; set; } = "A base keycard that is used to give a person a keycard with the right name and for custom colors via a command";
        public override float Weight { get; set; } = 0.01f;
        public override string KeycardLabel { get; set; } = "Great_Value";
        public override Color32? KeycardLabelColor { get; set; } = new(255, 255, 255, 255);
        public override string KeycardName { get; set; } = "Valued Shopper";
        public override Color32? KeycardPermissionsColor { get; set; } = new(0, 0, 0, 255);
        public override KeycardPermissions Permissions { get; set; } = KeycardPermissions.ContainmentLevelOne;
        public override Color32? TintColor { get; set; } = new(255, 255, 255, 255);
        public override ItemType Type { get; set; } = ItemType.KeycardCustomTaskForce;

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
    }
}