using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using PlayerRoles;
using UnityEngine;
using EP = Exiled.API.Features.Player;
using Item = Exiled.API.Features.Items.Item;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Room = Exiled.API.Features.Room;

namespace ExtendedItems
{
    public static class Utils
    {
        /// <summary>
        /// Calculates the global coords of a point inside a room based on the room type and the location
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="localPos"></param>
        /// <returns>Vector3</returns>
        public static Vector3 GetGlobalCords(RoomType roomType, Vector3 localPos)
        {
            var room = Room.Get(roomType);

            var rotation = room.Rotation;
            var roomPos = room.Position;

            var offsetY = Math.Round(Math.Abs(rotation.eulerAngles.y / 90f));

            return offsetY switch
            {
                0 => new Vector3(roomPos.x + localPos.x, roomPos.y + localPos.y, roomPos.z + localPos.z),
                1 => new Vector3(roomPos.x + localPos.z, roomPos.y + localPos.y, roomPos.z - localPos.x),
                2 => new Vector3(roomPos.x - localPos.x, roomPos.y + localPos.y, roomPos.z - localPos.z),
                3 => new Vector3(roomPos.x - localPos.z, roomPos.y + localPos.y, roomPos.z + localPos.x),
                _ => Vector3.zero,
            };
        }
        
        // ReSharper disable once InconsistentNaming
        public static bool PDWarning(EP player)
        {
            return (from actEffects in player.ActiveEffects
                    let Larry = EP.List.First(L => L.Role == RoleTypeId.Scp106).Position
                    select actEffects.name == "Corroding" && Plugin.Instance != null &&
                           Vector3.Distance(player.Position, Larry) < Plugin.Instance.Config.LarryDistance).FirstOrDefault();
        }

        public static void Exploding(EP player)
        {
            var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.1f;
            grenade.SpawnActive(player.Position + new Vector3(0, 1, 0), player);
        }

        public static bool HasEffect(EP player, EffectType effect)
        {
            return !Enum.IsDefined(typeof(EffectType), effect) ? throw new InvalidEnumArgumentException(nameof(effect), (int)effect, typeof(EffectType)) : player.ActiveEffects.Any(targetActiveEffect => targetActiveEffect.name == nameof(effect));
        }

        public static void Grenade_Damage(EffectGrenadeProjectile grenade, EP player)
        {
            if (Vector3.Distance(grenade.Position, player.Position) <= 4)
            {
                if (Physics.Raycast(grenade.Position, player.Position, out var _, 4, (int) LayerMasks.Grenade))
                {
                    if (player is { IsScp: true, HumeShield: > 900 })
                    {
                        player.Hurt(player, 900, armorPenetration: 50);
                    }
                    else if (player.IsHuman)
                    {
                        player.Hurt(grenade.PreviousOwner, 150, armorPenetration: 50);
                    }
                }
                
            }
            else if(Physics.Raycast(grenade.Position, player.Position, out var hit, 20, (int) LayerMasks.Grenade))
            {
                player.Hurt(player, 25, armorPenetration: 50);
            }
        }
        public static void Explode(Pickup? grenade, EP player)
        {
            if (grenade is null)
            {
                player.ShowHint("Shits Fucked up :(\nmb gng");
            }
            else
            {
                Vector3 lVoloc = grenade.Rigidbody.linearVelocity;
                Vector3 pos = grenade.Position;
                grenade.Destroy();
                var explosiveGrenade = (ExplosiveGrenade) Item.Create(ItemType.GrenadeHE, player);
                explosiveGrenade.FuseTime = Vector3.Distance(pos, player.Position) > 7f ? 1f : 5f;
        
                explosiveGrenade.Projectile.Rigidbody.linearVelocity = lVoloc;
                explosiveGrenade.SpawnActive(pos, player);
            }
        }
    }
}