using CommandSystem;
using Exiled.API.Features;
using ExtendedItems.Commands.GuideBook;

namespace ExtendedItems.Commands.Debug
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class Debug : ParentCommand
    {
        public Debug() { LoadGeneratedCommands(); }

        public static Debug Instance { get; } = new();
        public override string Command => "Debug";
        public override string[] Aliases => [ "db" ];
        public override string Description => "Overlord command for debugging the ExtendedItems plugin";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Locate.Instance);
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!sender.CheckPermission(Plugin.Instance!.Config.DebugPermissions))
            {
                response = "<color=red>Permission Denied.</color>";
                Log.Error($"{sender} tried to access admin commands!");
                return false;
            }

            response = "Invalid subcommand";
            return false;
        }
    }
}
