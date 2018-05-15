using KursV2.Helpers;
using System;

namespace KursV2.Interfaces
{
    interface ITuringMachine
    {
        int dimensions { get; }
        int qcount { get; }
        int q { get; }
        int tapeIndex { get; }
        int timelimit { get; }
        int memorylimit { get; }

        string addCommand(int q, string value, int nextq, string nextvalue, Moves move);
        string addCell(string value);
        string addCell(int index, string value);
        string run();
        string printCommands();
        string printTape();
    }
}