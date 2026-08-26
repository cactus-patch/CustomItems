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
using YamlDotNet.Serialization;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace ExtendedItems.Items;

[CustomItem(ItemType.SCP268)]
public class Scp1499 : CustomItem
{
    [Description("Room to teleport player to after using SCP-1499.")]
    // ReSharper disable once InconsistentNaming
    private const RoomType _teleportRoom = RoomType.Hcz106;

    [Description("Time for player to wander in seconds.")]
    // ReSharper disable once InconsistentNaming
    private const float _duration = 15f;

    [YamlIgnore] private readonly Dictionary<uint, (Vector3, Lift?, CoroutineHandle)> _lastPositions = [];

    [Description("Relative position of room mentioned above to teleport player to after using SCP-1499.")]
    private readonly Vector3 _relativePosition = new(5.75f, 10f, -10.75f);

    public override uint Id { get; set; } = 3;
    public override string Name { get; set; } = "SCP-1499";
    public override string Description { get; set; } = "<i>A breath away from oblivion.</i>";
    public override float Weight { get; set; } = 5f;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 0
    };

    private void OnUsingItem(UsingItemEventArgs ev)
    {
        if (!Check(ev.Item)) return;

        if (ev.Player.IsInPocketDimension || IsBlockedByPocketDimensionRules(ev.Player))
        {
            ev.Player.ShowHint(
                "You put on the gas mask but nothing happens.\nIt seems that this SCP item has its limits.");
            ev.IsAllowed = false;
            ev.Cooldown = 0.5f;
        }
    }

    private static bool IsBlockedByPocketDimensionRules(Player player)
    {
        if (Plugin.Instance is null || player.ActiveEffects.IsEmpty() ||
            player.ActiveEffects.All(effect => effect.name != "Corroding"))
            return false;

        var scp106 = Player.List.FirstOrDefault(rolePlayer => rolePlayer.Role == RoleTypeId.Scp106);
        return scp106 is not null &&
               Vector3.Distance(player.Position, scp106.Position) < Plugin.Instance.Config.LarryDistance;
    }

    private void OnUsedItem(UsedItemEventArgs ev)
    {
        if (!Check(ev.Item)) return;

        ev.Player.DisableEffect(EffectType.Invisible);
        ev.Player.EnableEffect(EffectType.DamageReduction, byte.MaxValue, _duration);

        var handle = Timing.CallDelayed(_duration, () => TeleportPrevious(ev.Player.NetId));

        _lastPositions.Add(ev.Player.NetId, (ev.Player.Position, ev.Player.Lift, handle));
        ev.Player.Teleport(Utils.GetGlobalCords(_teleportRoom, _relativePosition));
    }

    private void TeleportPrevious(uint netId)
    {
        if (!Player.TryGet(netId, out var player)) return;
        if (!_lastPositions.TryGetValue(netId, out var lastPos)) return;

        if (lastPos.Item2 != null)
            player.Teleport(lastPos.Item2.Position + Vector3.up * 2f);
        else
            player.Teleport(lastPos.Item1);

        _lastPositions.Remove(netId);
        Timing.KillCoroutines(lastPos.Item3);
    }

    protected override void SubscribeEvents()
    {
        PlayerEvents.UsedItem += OnUsedItem;
        PlayerEvents.UsingItem += OnUsingItem;

        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        PlayerEvents.UsedItem -= OnUsedItem;
        PlayerEvents.UsingItem -= OnUsingItem;

        base.UnsubscribeEvents();
    }

    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        if (!_lastPositions.ContainsKey(ev.Player.NetId)) return;

        ev.IsAllowed = false;
        TeleportPrevious(ev.Player.NetId);

        base.OnDroppingItem(ev);
    }
}
