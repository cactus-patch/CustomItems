using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace CustomItems
{
    public static class Utils {
        public static Vector3 GetGlobalCords(RoomType roomType, Vector3 localPos) {
            var room = Room.Get(roomType);
            var rotation = room.Rotation;
            var roomPos = room.Position;
            if (Math.Abs(rotation.eulerAngles.y) < 1.0)
            return new Vector3(roomPos.x + localPos.x, roomPos.y + localPos.y, roomPos.z + localPos.z);
            if (Math.Abs(rotation.eulerAngles.y - 90f) < 1.0)
            return new Vector3(roomPos.x + localPos.z, roomPos.y + localPos.y, roomPos.z - localPos.x);
            if (Math.Abs(rotation.eulerAngles.y - 180f) < 1.0)
            return new Vector3(roomPos.x - localPos.x, roomPos.y + localPos.y, roomPos.z - localPos.z);
            if (Math.Abs(rotation.eulerAngles.y - 270f) < 1.0)
            return new Vector3(roomPos.x - localPos.z, roomPos.y + localPos.y, roomPos.z + localPos.x);
            return Vector3.zero;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        /// <param name="minimum"></param>
        /// <returns></returns>
        public static bool TryRemoveItem(Player player, ItemType item, short minimum = 0)
        {
            if (player.CountItem(item) > minimum)
            {
                player.RemoveItem(player.Items.First(it => it.Type == item)); 
                return true;
            }
            return false;
        }

        public static ushort Subtrat(ushort inp)
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
    }
}