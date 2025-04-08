using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;
using UnityEngine;

namespace ExtendedItems
{
    public static class Utils {
        /// <summary>
        /// Calculates the global coords of a point inside a room based on the room type and the location
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="localPos"></param>
        /// <returns>Vector3</returns>
        public static Vector3 GetGlobalCords(RoomType roomType, Vector3 localPos) {
            var room = Room.Get(roomType);
            
            var rotation = room.Rotation;
            var roomPos = room.Position;
            
            double offsetY = Math.Round(Math.Abs(rotation.eulerAngles.y / 90f));

            return offsetY switch
            {
                0 => new Vector3(roomPos.x + localPos.x, roomPos.y + localPos.y, roomPos.z + localPos.z),
                1 => new Vector3(roomPos.x + localPos.z, roomPos.y + localPos.y, roomPos.z - localPos.x),
                2 => new Vector3(roomPos.x - localPos.x, roomPos.y + localPos.y, roomPos.z - localPos.z),
                3 => new Vector3(roomPos.x - localPos.z, roomPos.y + localPos.y, roomPos.z + localPos.x),
                _ => Vector3.zero
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
        public static bool TryRemoveItem(Player player, ItemType item, short minimum = 0)
        {
            if (player.CountItem(item) <= minimum) return false;
            
            player.RemoveItem(player.Items.First(it => it.Type == item)); 
            return true;
        }

        /// <summary>
        /// Removes 1 from a ushort (I hate this language sometimes)
        /// </summary>
        /// <param name="inp"></param>
        /// <returns>input - 1</returns>
        public static ushort Subtract(ushort inp)
        {
            int temp = inp;
            int m = 1;

            while (!((temp & m) > 0))
            {
                temp ^= m;
                m <<= 1;
            }

            temp ^= m;
            return (ushort)temp;
        }

        /// <summary>
        /// Creates a custom death message
        /// </summary>
        /// <param name="deathMessage"></param>
        /// <returns>CustomReasonDamageHandler</returns>
        public static CustomReasonDamageHandler CustomDeath(string deathMessage)
        {
            return new CustomReasonDamageHandler(deathMessage);
        }
    }
}