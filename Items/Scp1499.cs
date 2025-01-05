using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace CustomItems.Items;

[CustomItem(ItemType.SCP268)]
public class Scp1499 : CustomItem {
  public override uint Id { get; set; } = 420;
  public override string Name { get; set; } = "SCP-1499";
  public override string Description { get; set; } = "<i>A breath away from oblivion.</i>";
  public override float Weight { get; set; } = 5f;
  
  [Description("Position to teleport player to after using SCP-1499.")]
  public Vector3 Position { get; set; } = Vector3.zero;
  
  [Description("Time for player to wander in seconds.")]
  public float Duration { get; set; } = 5f;

  public override SpawnProperties? SpawnProperties { get; set; } = new() {
    Limit = 1,
    DynamicSpawnPoints = [
      new DynamicSpawnPoint() { Location = SpawnLocationType.InsideGateB }
    ]
  };

  private void OnUsingItem(UsingItemEventArgs ev) {
    if (!Check(ev.Item)) return;

    var previousPos = ev.Player.Position;
    var lift = Lift.List.First(lift => lift.IsInElevator(ev.Player.Position));
    ev.Player.Teleport(Position);

    Timing.CallDelayed(Duration, () => {
      ev.Player.EnableEffect(EffectType.DamageReduction, byte.MaxValue, 1f);
      if (lift != null) {
        ev.Player.Teleport(lift.Position + Vector3.up * 1.5f);
      }
      else {
        ev.Player.Teleport(previousPos);
      }
    });
  }

  protected override void SubscribeEvents() {
    PlayerEvents.UsingItem += OnUsingItem;
    
    base.SubscribeEvents();
  }

  protected override void UnsubscribeEvents() {
    PlayerEvents.UsingItem -= OnUsingItem;

    base.UnsubscribeEvents();
  }
  
  protected override void OnDropping(DroppingItemEventArgs ev) {
    if (!Check(ev.Item)) return;
    ev.IsAllowed = false;
    // todo: teleport player to previous pos if item is dropped
    
    base.OnDropping(ev);
  }
}