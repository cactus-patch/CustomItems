using CommandSystem;

namespace ExtendedItems.Commands.GuideBook
{
    internal sealed class Coinc : ICommand
    {
        private Coinc() { }

        public static Coinc Instance { get; } = new();
        public string Command => "coin";
        public string[] Aliases => ["1289"];
        public string Description => "Information on SCP-1289 (aka \"Coin\" or \"Colin\") ";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "<color=white>SCP-1289 (aka \"Coin\" or \"Colin\") is a $0.25 American coin that, when it lands on tails explodes the coin flipper.</color>\n";
            return true;
        }
    }
}

