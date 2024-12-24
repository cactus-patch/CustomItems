using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomModules.API.Features.Attributes;
using Exiled.CustomModules.API.Features.CustomItems;
using Exiled.CustomModules.API.Features.CustomItems.Items.Firearms;
using Exiled.CustomModules.API.Features.Generic;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomItems.Items;

public class Tranquilizer {
  [ModuleIdentifier]
  public class Item : CustomItem<Behaviour> {
    public override string Name => "Tranquilizer";
    public override uint Id { get; set; } = 1290;
    public override bool IsEnabled { get; set; } = true;
    public override string Description => "A gun that temporarily tranquilizes entities; might be unreliable.";
    public override ItemType ItemType => ItemType.GunCOM15;

    public override SettingsBase Settings => new Settings {
      PickedUpText = new TextDisplay($"You have picked up a <i>{Name}</i>.<br><i>{Description}</i>", channel: TextChannelType.Hint),
      SelectedText = new TextDisplay($"You have selected a <i>{Name}</i>.<br><i>{Description}</i>", channel: TextChannelType.Hint),
      NotifyItemToSpectators = true,
      SpawnProperties = new SpawnProperties {
        Limit = 1,
        DynamicSpawnPoints = [
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideLczCafe, Chance = 0.25f },
          new DynamicSpawnPoint { Location = SpawnLocationType.InsideLczWc, Chance = 0.25f, },
          new DynamicSpawnPoint { Position = Room.Get(RoomType.LczGlassBox).Position, Chance = 0.75f, }
        ]
      }
    };
  }
  
  public class Behaviour : FirearmBehaviour {
    protected override void OnShot(ShotEventArgs ev) {
      var rand = Random.value;
      var effective = ev.Player.IsScp ? rand > 0.5f : rand > 0.75f; // TODO: get values from config
      if ((ev.Player.Role == RoleTypeId.Scp173) || !effective) return;

      var lift = Lift.List.First(lift => lift.IsInElevator(ev.Player.Position));
      ev.Player.Scale = Vector3.zero;
      ev.Player.EnableEffect(EffectType.Ensnared, Byte.MaxValue);
      ev.Player.EnableEffect(EffectType.Flashed, Byte.MaxValue);
      ev.Player.EnableEffect(EffectType.Deafened, Byte.MaxValue);

      Timing.CallDelayed(5, () => {
        ev.Player.DisableEffect(EffectType.Ensnared);
        ev.Player.DisableEffect(EffectType.Flashed);
        ev.Player.DisableEffect(EffectType.Deafened);
        if (lift != null) ev.Player.Teleport(lift.Position + Vector3.up * 1.5f);
        ev.Player.Scale = Vector3.one;
      });
      
      base.OnShot(ev);
    }
  }
  
  [ModuleIdentifier]
  public class Config : ModulePointer<CustomItem> {
    public override uint Id { get; set; } = 1290;
    public bool EffectiveOn173 { get; set; } = false;
    public float SCPChance { get; set; } = 0.5f;
    public float HumanChance { get; set; } = 0.7f;
  }
}