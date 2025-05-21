using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Org.BouncyCastle.Utilities.Encoders;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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
        /// <param name="input"></param>
        /// <returns>input - 1</returns>
        public static ushort Subtract(ushort input)
        {
            int temp = input;
            int m = 1;
            while (!((temp & m) > 0)) { temp ^= m; m <<= 1; }
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

        /// <summary>
        /// Converts a hexadecimal color string to a <see cref="Color32"/> object.
        /// </summary>
        /// <remarks>This method attempts to parse the hexadecimal string into a <see cref="Color32"/>
        /// object. If the input is invalid or cannot be parsed, a default black color with full opacity is
        /// returned.</remarks>
        /// <param name="hex">A string representing the color in hexadecimal format. The string may optionally start with a '#' character
        /// and can be 2, 4, 6, or 8 characters long, representing different color components: <list type="bullet">
        /// <item><description>2 characters: Red component only (e.g., "FF").</description></item> <item><description>4
        /// characters: Red and Green components (e.g., "FF00").</description></item> <item><description>6 characters:
        /// Red, Green, and Blue components (e.g., "FF00FF").</description></item> <item><description>8 characters: Red,
        /// Green, Blue, and Alpha components (e.g., "FF00FF80").</description></item> </list></param>
        /// <returns>A <see cref="Color32"/> object representing the parsed color. If the input is null, empty, or invalid, the
        /// method returns a default black color with full opacity (<c>Color32(0, 0, 0, 255)</c>).</returns>
        public static Color32? Color(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return new Color32(0,0,0,255);

            hex = hex.TrimStart('#');

            if (hex.Length == 2)
            {
                try
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    return new Color32(r, 0, 0, 255);
                }
                catch
                {
                    return new Color32(0, 0, 0, 255);
                }
            }
            else if (hex.Length == 4)
            {
                try
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    return new Color32(r, g, 0, 255);
                }
                catch
                {
                    return new Color32(0, 0, 0, 255);
                }
            }
            else if (hex.Length == 6)

                try
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte a = 255;

                    if (hex.Length == 8)
                        a = Convert.ToByte(hex.Substring(6, 2), 16);

                    return new Color32(r, g, b, a);
                }
                catch
                {
                    return new Color32(0, 0, 0, 255);
                }
            else if (hex.Length == 8)
            {
                try
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte a = Convert.ToByte(hex.Substring(6, 2), 16);
                    return new Color32(r, g, b, a);
                }
                catch
                {
                    return new Color32(0, 0, 0, 255);
                }
            }
            else
            {
                return new Color32(0, 0, 0, 255);
            }
        }
    }
}