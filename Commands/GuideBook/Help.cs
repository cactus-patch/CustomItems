using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace ExtendedItems.Commands.GuideBook
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Help : ParentCommand
    {
        public Help() { LoadGeneratedCommands(); }
        public static Help Instance { get; } = new();
        public override string Command => "assistance";
        public override string[] Aliases => ["h"];
        public override string Description => "A parent command for helping with ExtendedItems";
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Coinc.Instance);
            RegisterCommand(GrenadeLauncher.Instance);
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: Coin, GrenadeLauncher, Plastic, SCP1162, SCP1499, Sniper, Tanq";
            return false;
        }
    }
}
