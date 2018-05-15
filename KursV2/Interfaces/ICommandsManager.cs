namespace KursV2.Interfaces
{
    interface ICommandsManager
    {
        int dimensions { get; }
        int qcount { get; }
        int q { get; }
        int commandsCounter { get; }

        string compile(string code);
    }
}