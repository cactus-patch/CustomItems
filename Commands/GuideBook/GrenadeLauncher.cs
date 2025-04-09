using CommandSystem;

namespace ExtendedItems.Commands.GuideBook
{
    internal sealed class GrenadeLauncher : ICommand
    {
        private GrenadeLauncher() { }
        public static GrenadeLauncher Instance { get; } = new();
        public string Command => "GrenadeLauncher";
        public string[] Aliases => ["GL"];
        public string Description => "Information on the Grenade Launcher ";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "A modified Logister that takes one (1) 7.62 bullet and one (1) High Explosive Grenade. ";
            return true;
        }
    }
}
