using KursV2.Helpers;
using KursV2.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace KursV2.Logic
{
    class TuringMachine : ITuringMachine
    {
        public int dimensions { get; private set; } = 1;
        public int qcount { get; private set; } = 3;
        public int q { get; private set; } = 0;
        public int[] tapeIndices { get; private set; }
        public int timelimit { get; private set; } = 10000;
        public int memorylimit { get; private set; } = 100;

        private Dictionary<string, Command> commands = new Dictionary<string, Command>(10);
        private Dictionary<string, string> tape = new Dictionary<string, string>(10);
        private long memoryUsed;
        private int operationsCounter = 0;
        private Stopwatch timeChecker = new Stopwatch();
        private int memorycheckfrequency;
        private Process currentProcess;
        private bool memoryFlag = false;

        public TuringMachine(int dimensions, int qcount, int startq, int timelimit, int memorylimit, int memorycheckfrequency)
        {
            this.dimensions = dimensions;
            this.qcount = qcount;
            q = startq;
            this.timelimit = timelimit;
            this.memorylimit = memorylimit;
            this.memorycheckfrequency = memorycheckfrequency;

            currentProcess = Process.GetCurrentProcess();
            memoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);

            tapeIndices = new int[dimensions];
        }

        public string addCommand(int q, string value, int nextq, string nextvalue, int dimension, Moves move)
        {
            if (!memoryFlag)
            {
                string key = q + "q" + value;

                if (!commands.ContainsKey(key))
                {
                    commands.Add(key, new Command(nextq, nextvalue, dimension, move));

                    ++operationsCounter;
                    if (operationsCounter > memorycheckfrequency)
                    {
                        if ((memoryUsed = currentProcess.WorkingSet64 / (1024 * 1024)) > memorylimit)
                        {
                            memoryFlag = true;

                            return "Error: Превышено допустимое количество памяти! / if then\\";
                        }
                        operationsCounter = 0;
                    }

                    return "Команда: / " + q + " / " + value + " /   ->   / " + nextq +
                        " / " + nextvalue + " / " + dimension + " / " + move + " / добавлена!\\";
                }
                else return "Error: Такая команда уже присутствует в машине! / if\\";
            }
            else return "Error: Превышено допустимое количество памяти! / if then\\";
        }

        public string addCell(int[] indices, string value)
        {
            if (!memoryFlag)
            {
                string key = string.Empty;
                foreach (var index in indices) key += index + "i";

                if (!tape.ContainsKey(key)) tape.Add(key, value);
                else tape[key] = value;

                ++operationsCounter;
                if (operationsCounter > memorycheckfrequency)
                {
                    if ((memoryUsed = currentProcess.WorkingSet64 / (1024 * 1024)) > memorylimit)
                    {
                        memoryFlag = true;

                        return "Error: Превышено допустимое количество памяти! / add\\";
                    }
                    operationsCounter = 0;
                }

                return value.ToString();
            }
            else return "Error: Превышено допустимое количество памяти! / add\\";
        }
        
        public string run()
        {
            if (memoryFlag) return "Error: Превышено допустимое количество памяти! / run\\";
            if (commands.Count == 0 || tape.Count == 0)
                return "Error: Необходимо добавить как минимум по одному значению на ленту и в список команд машины! / run\\";

            timeChecker.Start();
            double dif = 0;

            while (timelimit > (dif = timeChecker.Elapsed.TotalMilliseconds))
            {
                try
                {
                    string tapeKey = string.Empty;
                    string tapeValue = "@";
                    string commandsKey;

                    foreach (var index in tapeIndices) tapeKey += index + "i";
                    if (tape.ContainsKey(tapeKey))
                    {
                        tapeValue = tape[tapeKey];
                        commandsKey = q + "q" + tapeValue;
                    }
                    else commandsKey = q + "q@";

                    if (commands.ContainsKey(commandsKey))
                    {
                        Command command = commands[commandsKey];
                        q = command.q;
                        if (!tape.ContainsKey(tapeKey)) tape.Add(tapeKey, command.value);
                        else tape[tapeKey] = command.value;

                        switch (command.move)
                        {
                            case Moves.Left:
                                --tapeIndices[command.dimension - 1];
                                break;
                            case Moves.Right:
                                ++tapeIndices[command.dimension - 1];
                                break;
                            case Moves.Stop:
                                timeChecker.Reset();
                                return "Результат:\\Время выполнения (в миллисекундах): " + dif +
                                    "\\Занятая память (в мегабайтах): " + memoryUsed + "\\Конец результата!\\";
                        }
                    }
                    else return "Error: Не найдена команда: " + q + " | " + tapeValue + " / run\\";
                }
                catch { return "Error: Что-то пошло не так! / run\\"; }
            }

            return "Error: Превышено допустимое время ожидания! / run\\";
        }

        public string printCommands()
        {
            string result = string.Empty;

            foreach (var command in commands)
            {
                string[] key = StringHelpers.simpleSplit(command.Key, 'q');

                result += "Команда: / " + key[0] + " / " + key[1] + " /   ->   / " + command.Value.q +
                    " / " + command.Value.value + " / " + command.Value.dimension + " / " + command.Value.move + '\\';
            }

            return result;
        }

        public string printTape()
        {
            string result = "Лента: { / ";

            foreach (var value in tape)
            {
                result += value.Value + " / ";
            }
            result += "}\\";

            return result;
        }
    }
}