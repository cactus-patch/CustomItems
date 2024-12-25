using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomModules.API.Features.Attributes;
using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomItems.Items.Firearms;
using Exiled.CustomModules.API.Features.Generic;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace CustomItems.Items;

public class Sniper{
    [ModuleIdentifier]
    public class Item : CustomItem<Behaviour>{
        public override string Name => "SR-119";
        public override uint Id {get; set;} = 1291;
        public override float Weight {get; set;} = 4f;
        public override bool IsEnabled {get; set;} = true;
        public override string Description {get; set;} = "A modified E-11 that fires 5.56 at supersonic velocity that deals significantly more damage";
        public override float Damage {get; set;} = 7.5f;
        public override byte ClipSize {get; set;} = 1;
        public override ItemType ItemType => ItemType.GunE11SR;

        public override SettingsBase Settings => new FirearmSettings{
            PickedUpText = new TextDisplay($"You picked up <i>{Name}</i>,<br><i>{Description}</i>", channel: TextChannelType.Hint),
            SelectedText = new TextDisplay($"You have selected a <i>{Name}</i>.<br><i>{Description}</i>", channel: TextChannelType.Hint),
            NotifyItemToSpectators = true,
            SpawnProperties = new SpawnProperties {
            Limit = 1,
            DynamicSpawnPoints = [
                new DynamicSpawnPoint { Location = SpawnLocationType.InsideHczArmory, Chance = 1f }
            ]
            }
        };
    }

}