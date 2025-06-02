using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using System.Text;


namespace ExtendedItems.Commands.Debug
{
    internal sealed class Locate : ICommand
    {
        public static Locate Instance { get; } = new();
        public Locate() { }
        public string Command => "Locate";
        public string[] Aliases => ["loc", "find"];
        public string Description => "Locates all (unless a valid ID is given) Custom Items";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Plugin.Instance!.Config.DebugPermissions))
            {
                response = "Permission Denied";
                return false;
            }

            if (arguments.Count == 1)
            {
                if (!(uint.TryParse(arguments.At(0), out uint ID) && CustomItem.TryGet(ID, out CustomItem? item)) && !CustomItem.TryGet(arguments.At(0), out item))
                {
                    response = $"{arguments.At(0)} is not a valid custom item.";
                    return false;
                }

                var locations = new StringBuilder();
                foreach (CustomItem customItem in CustomItem.Registered)
                {
                    foreach (int inside in customItem.TrackedSerials)
                    {
                        Player owner = Player.List.FirstOrDefault(player => player.Inventory.UserInventory.Items.Any(item => item.Key == inside));
                        if (owner is null)
                        {

                        }
                    }
                }

                response = locations.Length > 0 ? locations.ToString() : "No items with the given ID were found.";
                return true;
            }
            response = "Not Implemented Yet.";
            // how tf do we implement this?

            if (arguments.Count <= 1)
            {
                return false;
            }

            return false;
        }
    }
}