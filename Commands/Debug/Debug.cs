using CommandSystem;

namespace ExtendedItems.Commands.Debug;
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class Debug : ParentCommand
    {
        public Debug() { LoadGeneratedCommands(); }

        public override string Command => "Debug";
        public override string[] Aliases => [ "db" ];
        public override string Description => "Overlord command for debugging the ExtendedItems plugin";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Debug.Locate.Instance);
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!sender.CheckPermission("extendeditems.debug"))
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
