using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
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
        Physics.Raycast(ev.Player.Position, ev.Player.Transform.forward, out var hit, 100f);

        var playerHit = Player.Get(hit.collider.gameObject);
        HitboxType? hitbox = hit.collider.gameObject.GetComponent<HitboxIdentity>().HitboxType;
        var doorHit = Door.Get(hit.collider.gameObject);

        if (doorHit is null && playerHit is null) return;

        if (playerHit is not null && Utils.PlayerShot[playerHit.Id])
        {
            Utils.PlayerShot[playerHit.Id] = false;
            playerHit.Kill(new CustomReasonDamageHandler("A Large bullet wound observed in the Head. Presumed to be " +
                                                         playerHit.Nickname));
            return;
        }

        // The switch case of fucking doom and despair

        switch (playerHit is not null)
        {
            case true when playerHit!.IsHuman:
                switch (hitbox)
                {
                    case Headshot when (HasEffect(playerHit, EffectType.AntiScp207) &&
                                        Plugin.Instance.Config.HeadRemove207) ||
                                       (playerHit.CurrentArmor.Type == ItemType.ArmorHeavy &&
                                        !Utils.PlayerShot[playerHit.Id]):

                        SavingPlayer(playerHit);
                        break;

                    case Limb or Body when HasEffect(playerHit, EffectType.AntiScp207) ||
                                           (playerHit.CurrentArmor.Type == ItemType.ArmorHeavy &&
                                            !Utils.PlayerShot[playerHit.Id]):
                        SavingPlayer(playerHit);
                        break;

                    default:
                        KillPlayer(playerHit, hitbox);
                        break;
                }

                break;

            case true when playerHit.IsScp:
                if (playerHit.Role == RoleTypeId.Scp0492 && hitbox == Headshot)
                    playerHit.Hurt(ev.Player, 300, armorPenetration: 75);
                else
                    playerHit.Hurt(ev.Player, 150, armorPenetration: 75);
                break;

            default:
                return;
        }

        base.OnShot(ev);
    }

    private static bool HasEffect(Player player, EffectType effectType)
    {
        return Enum.IsDefined(typeof(EffectType), effectType) &&
               player.ActiveEffects.Any(effect => effect.name == nameof(effectType));
    }

    private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
    {
        if (!Check(ev.Item)) return;

        // ReSharper disable once InconsistentNaming
        AttachmentName[] forcedSniperAttch = Plugin.Instance!.Config.ForcedSniperAttch;
        AttachmentName[] subForcedSniperAttch = ev.NewAttachmentIdentifiers.Select(user => user.Name).ToArray();

        // I hate this, but this is a way to do this, and I don't want to fuck with this anymore
        ev.IsAllowed = subForcedSniperAttch.All(forcedSniperAttch.Contains) &&
                       (forcedSniperAttch.Contains(AttachmentName.LowcapMagAP) ||
                        forcedSniperAttch.Contains(AttachmentName.LowcapMagJHP));
        ev.Player.AddAmmo(AmmoType.Nato556, (ushort)ev.Firearm.MagazineAmmo);
        ev.Firearm.MagazineAmmo = 0;
    }

    private static void SavingPlayer(Player player)
    {
        if (player.CurrentArmor.Type == ItemType.ArmorHeavy) Utils.PlayerShot[player.Id] = true;
        else player.DisableEffect(EffectType.AntiScp207);
        player.Health = 1;
        player.EnableEffect(EffectType.Invigorated, 10f);
        player.EnableEffect(EffectType.Concussed, 5f);
    }

    private static void KillPlayer(Player player, HitboxType? hitbox)
    {
        if (hitbox != null && !Enum.IsDefined(typeof(HitboxType), hitbox)) hitbox = Body;
        player.Kill(new CustomReasonDamageHandler("A Large bullet wound observed in the " + nameof(hitbox)));
        Respawn.GrantInfluence(player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 2);
        Respawn.AdvanceTimer(player.IsNTF ? Faction.FoundationStaff : Faction.FoundationEnemy, 8f);
    }
}