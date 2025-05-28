using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems;
using PlayerStatsSystem;
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
        public static bool TryRemoveItem(EP player, ItemType item, short minimum = 0)
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
        // I want someone to double check this before it goes into full prod
        public static ushort Subtract(ushort input, int m = 1)
        {
            int temp = input;
            int mask = m;
            while (!((temp & mask) > 0)) { temp ^= mask; mask <<= 1; }
            temp ^= mask;
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
        public static Color32 Hex2Color(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return new Color32(0, 0, 0, 255);

            hex = hex.TrimStart('#');
            byte g = 0, b = 0, a = 255;
            byte r;

            try
            {
                switch (hex.Length)
                {
                    case 2:
                        r = Convert.ToByte(hex.Substring(0, 2), 16);
                        break;
                    case 4:
                        r = Convert.ToByte(hex.Substring(0, 2), 16);
                        g = Convert.ToByte(hex.Substring(2, 2), 16);
                        break;
                    case 6:
                        r = Convert.ToByte(hex.Substring(0, 2), 16);
                        g = Convert.ToByte(hex.Substring(2, 2), 16);
                        b = Convert.ToByte(hex.Substring(4, 2), 16);
                        break;
                    case 8:
                        r = Convert.ToByte(hex.Substring(0, 2), 16);
                        g = Convert.ToByte(hex.Substring(2, 2), 16);
                        b = Convert.ToByte(hex.Substring(4, 2), 16);
                        a = Convert.ToByte(hex.Substring(6, 2), 16);
                        break;
                    default:
                        return new Color32(0, 0, 0, 255);
                }
            }
            catch
            {
                return new Color32(0, 0, 0, 255);
            }

            return new Color32(r, g, b, a);
        }

        // I finally got to use the ternary operator ٩( ๑╹ ꇴ╹)۶
        // I swear to god if this gets removed I will find you and beat you with a hammer
        // You can also see where I said fuck it and started naming methods like Microsoft
        public static ExItem.Item GetHeldOrFirst(EP player, ExItem.Item item) =>
            item = player.CurrentItem.Type.ToString().ToLower().Contains("keycard")
                ? player.CurrentItem : player.Items.FirstOrDefault(i => i.Type.ToString().ToLower().Contains("keycard"));

        [Obsolete("This isnt in use atm because NW fucked up Custom key cards")]
        public static void CustomKeycardSetup(CK.CustomKeycard keycard, string name, string label, string labelColor, string permissionsColor, string tintColor)
        {
            keycard.KeycardName = name;
            keycard.KeycardLabel = label;
            keycard.KeycardLabelColor = Hex2Color(labelColor);
            keycard.KeycardPermissionsColor = Hex2Color(permissionsColor);
            keycard.TintColor = Hex2Color(tintColor);
        }

        public static List<string> GetPropertiesOfHeldOrFirst(EP player)
        {
            List<string> response = [];
            ExItem.Item helditem = GetHeldOrFirst(player, player.Items.FirstOrDefault(i => i.Type == ItemType.KeycardFacilityManager));
            helditem.CopyProperties(response);
            return response;
        }

        

    }
}