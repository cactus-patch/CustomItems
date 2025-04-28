using CommandSystem;
/*using Exiled.CustomItems.API.Features;*/

namespace ExtendedItems.Commands.Debug
{
    internal sealed class Locate : ICommand
    {
        public static Locate Instance { get; } = new();
        public Locate() {}
        public string Command => "Locate";
        public string[] Aliases => ["loc", "find"];
        public string Description => "Locates all (unless a valid ID is given) Custom Items";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(ExtendedItems.Plugin.Instance!.Config.DebugPermissions))
            {
                response = "Permission Denied";
                return false;
            }

            response = "Not Implemented Yet.";
            
            if (arguments.Count <= 1)
            {
                return false;
            }

            return false;
        }
    }
}