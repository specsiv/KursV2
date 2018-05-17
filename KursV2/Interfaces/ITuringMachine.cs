using KursV2.Helpers;

namespace KursV2.Interfaces
{
    interface ITuringMachine
    {
        int dimensions { get; }
        int qcount { get; }
        int q { get; }
        int[] tapeIndices { get; }
        int timelimit { get; }
        int memorylimit { get; }

        string addCommand(int q, string value, int nextq, string nextvalue, int dimension, Moves move);
        string addCell(int[] indices, string value);
        string run();
        string printCommands();
        string printTape();
    }
}