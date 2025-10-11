using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;
using EP = Exiled.API.Features.Player;

namespace ExtendedItems
{
    public static class Utils
    {
        /// <summary>
        ///     Calculates the global coords of a point inside a room based on the room type and the location
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

        /// <summary>
        ///     Removes 1 from an ushort (I hate this language sometimes)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mask"></param>
        /// <returns>input - 1</returns>
        // I want someone to double-check this before it goes into full prod
        public static ushort Subtract(ushort input, int mask = 1)
        {
            int temp = input;
            while (!((temp & mask) > 0))
            {
                temp ^= mask;
                mask <<= 1;
            }

            temp ^= mask;
            return (ushort)temp;
        }

        // ReSharper disable once InconsistentNaming
        public static bool PDWarning(EP player)
        {
            return (from actEffects in player.ActiveEffects let Larry = EP.List.First(L => L.Role == RoleTypeId.Scp106).Position select actEffects.name == "Corroding" && Plugin.Instance != null && Vector3.Distance(player.Position, Larry) < Plugin.Instance.Config.LarryDistance).FirstOrDefault();
        }

        public static void Exploding(EP player)
        {
            var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.1f;
            grenade.SpawnActive(player.Position + new Vector3(0, 1, 0), player);
        }
    }
}