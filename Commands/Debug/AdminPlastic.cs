using CommandSystem;
using Exiled.API.Features;

namespace ExtendedItems.Commands.Debug
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class PlasticDebug : ICommand
    {

        public string Command => "Ddetonate";

        public string[] Aliases => ["Ddet", "Dd", "Dboom"];

        public string Description => "Detonate command for C4 charges that you have placed";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);
            if (!sender.CheckPermission(Plugin.Instance!.Config.DebugPermissions))
            {
                if (!Items.Plastic.PlacedCharges.ContainsValue(ply))
                {
                    response = "\n<color=red>You've haven't placed any C4 charges!</color>";
                    return false;
                }

                int i = 0;

                foreach (var charge in Items.Plastic.PlacedCharges.ToList())
                {
                    if (charge.Value != ply) continue;

                    Items.Plastic.Instance.Handler(charge.Key, Items.Plastic.C4RemoveMethod.Detonate);
                    i++;
                }

                response = i == 1 ? $"\n<color=green>{i} C4 charge has been detonated!</color>" : $"\n<color=green>{i} C4 charges have been detonated!</color>";
                return true;
            }
            response = "<color=red>Permission Denied.</color>";
            return false;
        }
    }
}
