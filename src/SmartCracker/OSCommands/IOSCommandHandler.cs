namespace SmartCracker.OSCommands
{
    public interface IOSCommandHandler
    {
         string Run(string command, bool interactive = false, bool displayLive = false);
    }
}