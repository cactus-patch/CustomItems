using CommandSystem;
using Exiled.API.Features;
using Utf8Json.Resolvers.Internal;

namespace ExtendedItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Plastic : ICommand
    {
        public string Command => "detonate";

        public string[] Aliases => ["det", "d", "boom"];

        public string Description => "Detonate command for C4 charges that you have placed";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);
            if (!Items.Plastic.PlacedCharges.ContainsValue(ply))
            {
                response = "\n<color=red>You've haven't placed any C4 charges!</color>";
                return false;
            }

            int i = 0;
            foreach (var charge in Items.Plastic.PlacedCharges.ToList())
            {
                float posy =  charge.Key.Position.y;
                if (charge.Value != ply) continue;
                
                if (ply.Position.y >= posy - 100 && ply.Position.y <= posy + 100 )
                {
                    Utils.GlobalDet = Player.Get(sender);
                    Items.Plastic.Instance.Handler(charge.Key, Items.Plastic.C4RemoveMethod.Detonate, Player.Get(sender));
                    i++;
                }
                else
                {
                    ply.SendConsoleMessage($"One of your charges is out of range. You need to get within the zone that it was placed", "yellow");
                }
            }

            response = i == 1 ? $"\n<color=green>{i} C4 charge has been detonated!</color>" : $"\n<color=green>{i} C4 charges have been detonated!</color>";
            return true;
        }
    }
}
