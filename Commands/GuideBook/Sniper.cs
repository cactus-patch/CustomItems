using CommandSystem;

namespace ExtendedItems.Commands.GuideBook
{
    internal sealed class Sniper : ICommand
    {
        private Sniper() { }

        public static Sniper Instance { get; } = new();
        public string Command => "Sniper";
        public string[] Aliases => ["SR-118"];
        public string Description => "Information on the Sniper";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "<color=white>A modified E-11 that fires a singular 5.56 that deals ~118 damage per shot</color>\n";
            return true;
        }
    }
}

