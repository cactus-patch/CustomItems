using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace CustomItems.Items;

[CustomItem(ItemType.SCP268)]
public class Scp1499 : CustomItem {
  public override uint Id { get; set; } = 420;
  public override string Name { get; set; } = "SCP-1499";
  public override string Description { get; set; } = "<i>A breath away from oblivion.</i>";
  public override float Weight { get; set; } = 5f;
  
  [Description("Room and its relative position to teleport player to after using SCP-1499.")]
  public (RoomType, Vector3) Position { get; set; } = (RoomType.Hcz106, new Vector3(5.75f, 10f, -10.75f));
  
  [Description("Time for player to wander in seconds.")]
  public float Duration { get; set; } = 15f;

  [YamlIgnore] private Dictionary<uint, (Vector3, Lift?, CoroutineHandle)> _lastPositions = [];

  public override SpawnProperties? SpawnProperties { get; set; } = new() {
    Limit = 1,
    DynamicSpawnPoints = [
      new DynamicSpawnPoint() { Location = SpawnLocationType.InsideSurfaceNuke }
    ]
  };

  private void OnUsingItem(UsingItemEventArgs ev) {
    if (!Check(ev.Item) || !ev.IsAllowed || ev.Cooldown > 0) return;
    Timing.CallDelayed(1.5f, () => {
      ev.Player.DisableEffect(EffectType.Invisible);
      ev.Player.EnableEffect(EffectType.DamageReduction, byte.MaxValue, Duration);

      var handle = Timing.CallDelayed(Duration, () => TeleportPrevious(ev.Player.NetId));
      _lastPositions.Add(ev.Player.NetId, (ev.Player.Position, ev.Player.Lift, handle));
      ev.Player.Teleport(Utils.GetGlobalCords(Position));
    });
  }

  private void TeleportPrevious(uint netId) {
    if (!Player.TryGet(netId, out var player)) return;
    if (!_lastPositions.TryGetValue(netId, out var lastPos)) return;
    
    if (lastPos.Item2 != null) {
      player.Teleport(lastPos.Item2.Position + Vector3.up * 1.5f);
    }
    else {
      player.Teleport(lastPos.Item1);
    }

    _lastPositions.Remove(netId);
    Timing.KillCoroutines(lastPos.Item3);
  }

  protected override void SubscribeEvents() {
    PlayerEvents.UsingItem += OnUsingItem;
    
    base.SubscribeEvents();
  }

  protected override void UnsubscribeEvents() {
    PlayerEvents.UsingItem -= OnUsingItem;

    base.UnsubscribeEvents();
  }
  
  protected override void OnDroppingItem(DroppingItemEventArgs ev) {
    if (!Check(ev.Item)) return;
    if (!_lastPositions.ContainsKey(ev.Player.NetId)) return;
    ev.IsAllowed = false;
    TeleportPrevious(ev.Player.NetId);

    ev.Player.DropItem(ev.Item);
    base.OnDroppingItem(ev);
  }
}