using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;
using static HitboxType;
using ItemEvents = Exiled.Events.Handlers.Item;

namespace ExtendedItems.Items;

[CustomItem(ItemType.GunE11SR)]
public class Sniper : CustomWeapon
{
    public override uint Id { get; set; } = 2;
    public override string Name { get; set; } = "SR-118";

    public override string Description { get; set; } =
        "A modified E-11 that fires 5.56 at supersonic velocity that deals significantly more damage";

    public override float Weight { get; set; } = 4f;
    public override float Damage { get; set; } = 0f;
    public override byte ClipSize { get; set; } = 1;

    [YamlIgnore]
    public override AttachmentName[] Attachments { get; set; } =
    [
        AttachmentName.LowcapMagAP,
        AttachmentName.Foregrip,
        AttachmentName.DotSight,
        AttachmentName.RecoilReducingStock,
        AttachmentName.RifleBody,
        AttachmentName.MuzzleBrake
    ];
    
    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        LockerSpawnPoints =
        [
            new LockerSpawnPoint
            {
                Type = LockerType.RifleRack,
                Chance = 100,
                UseChamber = true, 
                Zone = ZoneType.HeavyContainment
            }
        ]
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

    protected override void OnShot(ShotEventArgs ev)
    {
        if (!Check(ev.Item) || Plugin.Instance is null || ev.Target is null) return;
        ev.CanHurt = false;
        var raycast = Physics.Raycast(ev.Player.Position, ev.Player.Transform.forward, 100f,
            LayerMask.GetMask("OnlyWorldOnlyWorldCollision", "Player"));
        switch (raycast)
        {
            // ev.Target.Kill(new CustomReasonDamageHandler("A Large bullet wound observed in the head of " + ev.Target.Nickname));
            // Respawn.GrantInfluence(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 2);
            // Respawn.AdvanceTimer(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 8f);
            case true when ev.Target.IsHuman:
                switch (ev.Hitbox.HitboxType)
                {
                    case Headshot or Limb or Body when HasEffect(ev.Target, EffectType.AntiScp207) &&
                                                       Plugin.Instance.Config.HeadRemove207:
                        ev.Target.DisableEffect(EffectType.AntiScp207);
                        ev.Target.Health = 1;
                        ev.Target.EnableEffect(EffectType.Concussed, 10f);
                        ev.Target.EnableEffect(EffectType.Invigorated, 5f);
                        break;

                    case Headshot or Body or Limb when ev.Target.CurrentArmor.Type == ItemType.ArmorHeavy &&
                                                       !HasEffect(ev.Target, EffectType.AntiScp207):
                        ev.Target.Health = 1;
                        ev.Target.EnableEffect(EffectType.Concussed, 10f);
                        ev.Target.EnableEffect(EffectType.Invigorated, 5f);
                        break;

                    case Headshot:
                        ev.Target.Kill(new CustomReasonDamageHandler(
                            "A Large bullet wound observed in the Head. Presumed to be " + ev.Target.Nickname));
                        Respawn.GrantInfluence(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 2);
                        Respawn.AdvanceTimer(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 8f);
                        break;

                    case Body:
                        ev.Target.Kill(new CustomReasonDamageHandler("A Large bullet wound observed in the Body of " +
                                                                     ev.Target.Nickname));
                        Respawn.GrantInfluence(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 2);
                        Respawn.AdvanceTimer(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 8f);
                        break;
                    default:
                        ev.Target.Kill(new CustomReasonDamageHandler(
                            "A Large section of flesh in an extremity observed on " + ev.Target.Nickname));
                        Respawn.GrantInfluence(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 2);
                        Respawn.AdvanceTimer(ev.Player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 8f);
                        break;
                }

                break;

            case true when ev.Target.IsScp:
                if (ev.Target.Role == RoleTypeId.Scp0492 && ev.Hitbox.HitboxType == Headshot)
                    ev.Target.Hurt(ev.Player, 300, armorPenetration: 75);
                else
                    ev.Target.Hurt(ev.Player, 150, armorPenetration: 75);
                break;
        }

        base.OnShot(ev);
    }

    private static bool HasEffect(Player player, EffectType effectType)
    {
        return !Enum.IsDefined(typeof(EffectType), effectType)
            ? throw new InvalidEnumArgumentException(nameof(effectType), (int)effectType, typeof(EffectType))
            : player.ActiveEffects.Any(effect => effect.name == nameof(effectType));
    }

    private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
    {
        if (!Check(ev.Item)) return;

        // ReSharper disable once InconsistentNaming
        AttachmentName[] Monica = Plugin.Instance!.Config.ForcedSniperAttch;
        AttachmentName[] subMonica = ev.NewAttachmentIdentifiers.Select(user => user.Name).ToArray();

        // I hate this, but this is a way to do this, and I don't want to fuck with this anymore
        ev.IsAllowed = subMonica.All(Monica.Contains) &&
                       (Monica.Contains(AttachmentName.LowcapMagAP) || Monica.Contains(AttachmentName.LowcapMagJHP));
        ev.Player.AddAmmo(AmmoType.Nato556, (ushort)ev.Firearm.MagazineAmmo);
        ev.Firearm.MagazineAmmo = 0;
    }
}