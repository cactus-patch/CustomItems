using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace ExtendedItems.Items;

[CustomItem(ItemType.GunLogicer)]
public class GrenadeLauncher : CustomWeapon
{
    private static bool _grenadeLaunchCoroutine;
    public override uint Id { get; set; } = 5;
    public override string Name { get; set; } = "Grenade Launcher";

    public override string Description { get; set; } =
        "A modified Chaos Insurgency LMG that fires High Explosive Grenades";

    public override float Weight { get; set; } = 10f;
    public override float Damage { get; set; } = 0f;
    public override byte ClipSize { get; set; } = 1;


    [YamlIgnore]
    public override AttachmentName[] Attachments { get; set; } =
        [AttachmentName.Laser, AttachmentName.IronSights, AttachmentName.ShortBarrel];

    public override SpawnProperties? SpawnProperties { get; set; } = new()

    {
        Limit = 1,
        LockerSpawnPoints =
        [
            new LockerSpawnPoint
            {
                Chance = 100,
                Type = LockerType.ExperimentalWeapon,
                UseChamber = true
            }
        ]
    };

    protected override void OnShooting(ShootingEventArgs ev)
    {
        bool? hasSpawn = TrySpawn(10, ev.Player.Position + new Vector3(0f, .25f, 0f), out var pickupBase);

        if (hasSpawn is true)
        {
            if (!_grenadeLaunchCoroutine)
            {
                var thrownProjectile = pickupBase!.GameObject.AddComponent<ThrownProjectile>();
                pickupBase.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, thrownProjectile);
                pickupBase.Rigidbody.linearVelocity = new Vector3(3f, .05f);
                if (!_grenadeLaunchCoroutine)
                {
                    _grenadeLaunchCoroutine = true;
                    Timing.RunCoroutine(GrenadeLaunch(pickupBase));
                }
            }
            else
            {
                ev.Player.ShowHint("Cant shoot yet", 5f);
            }
        }
        else
        {
            ev.Player.ShowHint("Something messed up", 5f);
        }


        base.OnShooting(ev);
    }

    protected override void OnChangingAttachment(ChangingAttachmentsEventArgs ev)
    {
        if (ev.NewAttachmentIdentifiers.Any(attachment => attachment.Name == AttachmentName.ShortBarrel))
        {
            ev.IsAllowed = true;
        }
        else
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint("You need a Short Barrel because I said so :) -Noobest1001", 5f);
        }

        base.OnChangingAttachment(ev);
    }

    private static IEnumerator<float> GrenadeLaunch(Pickup pickup)
    {
        if (pickup is not GrenadePickup) yield break;
        while (true)
        {
            yield return Timing.WaitForSeconds(.5f);

            if (pickup.Rigidbody.linearVelocity.magnitude < .5f)
            {
                Utils.Explode(pickup, pickup.PreviousOwner);
                _grenadeLaunchCoroutine = false;
                yield break;
            }
        }
    }
}