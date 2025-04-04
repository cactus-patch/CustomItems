using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;
using YamlDotNet.Core.Tokens;

namespace ExtendedItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class PlasticDebug : ICommand
    {
        public string Command => "Ddetonate";

        public string[] Aliases => ["Ddet", "Dd", "Dboom"];

        public string Description => "Detonate command for C4 charges that you have placed";

        public enum Zone { Surface = 1000, Lcz = 0, Hcz = -1000 }
        private const string RequiredPermission = "ExtendedItems.Debug";

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
                float posy = charge.Key.Position.y;
                if (charge.Value != ply)
                    continue;

                Items.Plastic.Instance.Handler(charge.Key, Items.Plastic.C4RemoveMethod.Detonate);

                i++;
            }

            response = i == 1 ? $"\n<color=green>{i} C4 charge has been detonated!</color>" : $"\n<color=green>{i} C4 charges have been deonated!</color>";

            return true;
        }
    }
}
