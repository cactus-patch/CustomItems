using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.Handlers;
using InventorySystem.Items.Usables.Scp330;
using PlayerRoles;
using UnityEngine;
using CK = Exiled.CustomItems.API.Features;
using EP = Exiled.API.Features.Player;
using ExItem = Exiled.API.Features.Items; // Exiled.API.Features.Items not ExtededItems

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
            Room room = Room.Get(roomType);

            Quaternion rotation = room.Rotation;
            Vector3 roomPos = room.Position;

            double offsetY = Math.Round(Math.Abs(rotation.eulerAngles.y / 90f));

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
        /// Trys to remove an item from a players inventory
        /// returns true if the item was removed
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="minimum"></param>
        /// <example> 
        /// if(Utils.TryRemoveItem(ev.Player, ItemType.GrenadeHE))
        /// </example>
        /// <returns>bool</returns>
        public static bool TryRemoveItem(EP player, ItemType item, short minimum = 0)
        {
            if (player.CountItem(item) <= minimum) return false;

            player.RemoveItem(player.Items.First(it => it.Type == item));
            return true;
        }

        /// <summary>
        /// Removes 1 from an ushort (I hate this language sometimes)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mask"></param>
        /// <returns>input - 1</returns>
        // I want someone to double-check this before it goes into full prod
        
        public static ushort Subtract(ushort input, int mask = 1)
        {
            int temp = input;
            while (!((temp & mask) > 0)) { temp ^= mask; mask <<= 1; }
            temp ^= mask;
            return (ushort)temp;
        }
    }
}