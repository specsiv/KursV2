using KursV2.Helpers;
using KursV2.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace KursV2.Logic
{
    class TuringMachine : ITuringMachine
    {
        public int dimensions { get; private set; } = 2;
        public int qcount { get; private set; } = 3;
        public int q { get; private set; } = 0;
        public int tapeIndex { get; private set; } = 0;
        public int timelimit { get; private set; } = 10000;
        public int memorylimit { get; private set; } = 100;

        private Dictionary<string, Command> commandsDic = new Dictionary<string, Command>(10);
        private List<string> tape = new List<string>(10);
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
        }

        public string addCommand(int q, string value, int nextq, string nextvalue, Moves move)
        {
            if (!memoryFlag)
            {
                string key = q + "q" + value;

                if (!commandsDic.ContainsKey(key))
                {
                    commandsDic.Add(key, new Command(nextq, nextvalue, move));

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
                        " / " + nextvalue + " / " + move + " / добавлена!\\"; ;
                }
                else return "Error: Такая команда уже присутствует в машине! / if\\";
            }
            else return "Error: Превышено допустимое количество памяти! / if then\\";
        }

        //public bool addCommands(int q, List<string> values, int nextq, string nextvalue, Moves move)
        //{
        //    string key;

        //    foreach (var v in values)
        //    {
        //        key = q + "q" + v;

        //        if (!commandsDic.ContainsKey(key))
        //        {
        //            commandsDic.Add(key, new Command(nextq, nextvalue, move));
        //        }
        //        else return false;
        //    }

        //    return true;
        //}

        //public bool deleteCommand(string key)
        //{
        //    if (commandsDic.ContainsKey(key))
        //    {
        //        commandsDic.Remove(key);

        //        return true;
        //    }
        //    else return false;
        //}

        public string addCell(string value)
        {
            if (!memoryFlag)
            {
                tape.Add(value);

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

        public string addCell(int index, string value)
        {
            if (!memoryFlag)
            {
                if (index < tape.Count)
                {
                    tape.Insert(index, value);

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
                else return "Error: Некорректный индекс! / add\\";
            }
            else return "Error: Превышено допустимое количество памяти! / add\\";
        }

        //public void addCells(List<string> values)
        //{
        //    tape.AddRange(values);
        //}

        //public void deleteCell(int index)
        //{
        //    tape.RemoveAt(index);
        //}

        public string run()
        {
            if (memoryFlag) return "Error: Превышено допустимое количество памяти! / run\\";
            if (commandsDic.Count == 0 || tape.Count == 0)
                return "Error: Необходимо добавить как минимум по одному значению на ленту и в список команд машины! / run\\";

            timeChecker.Start();
            double dif = 0;

            while (timelimit > (dif = timeChecker.Elapsed.TotalMilliseconds))
            {
                try
                {
                    string key = q + "q" + tape[tapeIndex];

                    if (commandsDic.ContainsKey(key))
                    {
                        Command command = commandsDic[key];
                        q = command.q;
                        tape[tapeIndex] = command.value;

                        switch (command.move)
                        {
                            case Moves.Left:
                                if (tapeIndex > 0) --tapeIndex;
                                else return "Error: Индекс ленты не может быть отрицательным! / run\\";
                                break;
                            case Moves.Right:
                                if (tapeIndex < tape.Count - 1) ++tapeIndex;
                                else return "Error: Индекс ленты не может быть больше длины ленты минус 1! / run\\";
                                break;
                            case Moves.Stop:
                                timeChecker.Reset();
                                return "Результат:\\Время выполнения (в миллисекундах): " + dif +
                                    "\\Занятая память (в мегабайтах): " + memoryUsed + "\\Конец результата!\\";
                        }
                    }
                    else return "Error: Не найдена команда: " + q + " | " + tape[tapeIndex] + " / run\\";
                }
                catch { return "Error: Что-то пошло не так! / run\\"; }
            }

            return "Error: Превышено допустимое время ожидания! / run\\";
        }

        public string printCommands()
        {
            string result = string.Empty;

            foreach (var command in commandsDic)
            {
                string[] key = StringHelpers.simpleSplit(command.Key, 'q');

                result += "Команда: / " + key[0] + " / " + key[1] + " /   ->   / " + command.Value.q +
                    " / " + command.Value.value + " / " + command.Value.move + '\\';
            }

            return result;
        }

        public string printTape()
        {
            string result = "Лента: { / ";

            foreach (var value in tape)
            {
                result += value + " / ";
            }
            result += "}\\";

            return result;
        }
    }
}