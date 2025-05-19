using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace ExtendedItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class Main : ParentCommand
    {
        public Main() { LoadGeneratedCommands(); }

        public override string Command => "ExtendedItems";
        public override string[] Aliases => [ "ei", "extendeditems", "eis" ];
        public override string Description => "Overlord command for the ExtendedItems plugin";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(GuideBook.Help.Instance);
            RegisterCommand(Debug.Debug.Instance);
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: help";
            return false;
        }
    }
}
