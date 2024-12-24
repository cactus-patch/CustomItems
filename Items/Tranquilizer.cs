using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomItems.Items;

[CustomItem(ItemType.GunCOM15)]
public class Tranquilizer : CustomWeapon {
    public override string Name { get; set; } = "Tranquilizer";
    public override uint Id { get; set; } = 1290;
    public override string Description { get; set; } = "A gun that temporarily tranquilizes entities; might be unreliable.";
    public override float Weight { get; set; } = 1f;
    public override float Damage { get; set; } = 1f;
    public override byte ClipSize { get; set; } = 3;
    
    [Description("Whether the tranquilizer is effective on SCP-173.")]
    public bool EffectiveOn173 { get; set; } = false;
    
    [Description("The effectiveness of tranquilizer on SCPs in decimal percentage.")]
    public float ScpChance { get; set; } = 0.5f;
    
    [Description("The effectiveness of tranquilizer on humans in decimal percentage.")]
    public float HumanChance { get; set; } = 0.75f;

    public override SpawnProperties? SpawnProperties { get; set; } = new() {
      Limit = 1,
      DynamicSpawnPoints = [
        new DynamicSpawnPoint { Chance = 25, Location = SpawnLocationType.InsideLczCafe },
        new DynamicSpawnPoint { Chance = 25, Location = SpawnLocationType.InsideLczWc },
        new DynamicSpawnPoint { Chance = 75, Position = Room.Get(RoomType.LczGlassBox).Position }
      ],
    };
  
    protected override void OnShot(ShotEventArgs ev) {
      var rand = Random.value;
      var effective = ev.Player.IsScp ? rand < ScpChance : rand < HumanChance;
      if ((ev.Player.Role == RoleTypeId.Scp173) || !effective) return;

      var lift = Lift.List.First(lift => lift.IsInElevator(ev.Player.Position));
      ev.Player.Scale = Vector3.zero;
      ev.Player.EnableEffect(EffectType.Ensnared, byte.MaxValue);
      ev.Player.EnableEffect(EffectType.Flashed, byte.MaxValue);
      ev.Player.EnableEffect(EffectType.Deafened, byte.MaxValue);

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