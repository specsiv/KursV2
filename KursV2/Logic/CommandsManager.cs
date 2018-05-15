using KursV2.Helpers;
using KursV2.Interfaces;

namespace KursV2.Logic
{
    class CommandsManager : ICommandsManager
    {
        public int dimensions
        {
            get
            {
                if (turingMachine == null) return -1;
                else return turingMachine.dimensions;
            }

            private set { }
        }

        public int qcount
        {
            get
            {
                if (turingMachine == null) return -1;
                else return turingMachine.qcount;
            }

            private set { }
        }

        public int q
        {
            get
            {
                if (turingMachine == null) return -1;
                else return turingMachine.q;
            }

            private set { }
        }

        public int timelimit
        {
            get
            {
                if (turingMachine == null) return -1;
                else return turingMachine.timelimit;
            }

            private set { }
        }

        public int memorylimit
        {
            get
            {
                if (turingMachine == null) return -1;
                else return turingMachine.memorylimit;
            }

            private set { }
        }

        public int commandsCounter { get; private set; } = 1;

        private ITuringMachine turingMachine = null;

        private string init(string[] keyWords)
        {
            int dimensions = 2;
            int qcount = 3;
            int startq = 0;
            int timelimit = 10000;
            int memorylimit = 100;
            int memorycheckfrequency = 10;

            for (int i = 1; i < keyWords.Length; ++i)
            {
                switch (keyWords[i])
                {
                    case "d":
                    case "dimensions":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра dimensions! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out dimensions) && dimensions > 1))
                            return "Error: Значением параметра dimensions должно быть целое положительное число, большее 1! / init\\";

                        ++i;
                        break;
                    case "qc":
                    case "qcount":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра qcount! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out qcount) && qcount > 0))
                            return "Error: Значением параметра qcount должно быть целое положительное число! / init\\";

                        ++i;
                        break;
                    case "sq":
                    case "startq":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра startq! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out startq) && startq >= 0))
                            return "Error: Значением параметра startq должно быть целое положительное число или 0! / init\\";

                        ++i;
                        break;
                    case "tl":
                    case "timelimit":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра timelimit! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out timelimit) && timelimit > 0))
                            return "Error: Значением параметра timelimit должно быть целое положительное число! / init\\";

                        ++i;
                        break;
                    case "ml":
                    case "memorylimit":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра memorylimit! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out memorylimit) && memorylimit > 0))
                            return "Error: Значением параметра memorylimit должно быть целое положительное число! / init\\";

                        ++i;
                        break;
                    case "mcf":
                    case "memorycheckfrequency":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра memorycheckfrequency! / init\\";
                        if (!(int.TryParse(keyWords[i + 1], out memorycheckfrequency) && memorycheckfrequency > 0))
                            return "Error: Значением параметра memorycheckfrequency должно быть целое положительное число! / init\\";

                        ++i;
                        break;
                }
            }

            if (startq >= qcount) return "Error: Значение параметра startq должно быть меньше количества состояний! / init\\";
            turingMachine = new TuringMachine(dimensions, qcount, startq, timelimit, memorylimit, memorycheckfrequency);

            return "Инициализация прошла успешно!\\";
        }

        private string clear()
        {
            turingMachine = null;

            return "Отчистка прошла успешно!\\";
        }

        private string ifThen(string[] keyWords)
        {
            if (turingMachine == null) return "Error: Необходимо сначала инициализировать машину! / if\\";

            int thenIndex;
            int currentq = 0;
            int nextq = qcount;
            string currentvalue = string.Empty;
            string nextvalue = string.Empty;
            Moves move = Moves.StayHere;

            for (thenIndex = 1; thenIndex < keyWords.Length; ++thenIndex)
            {
                if (keyWords[thenIndex].Equals("then")) break;
            }
            if (thenIndex >= keyWords.Length) return "Error: Не найдено ключевое слово then! / if\\";

            for (int i = 1; i < thenIndex; ++i)
            {
                switch (keyWords[i])
                {
                    case "q":
                    case "currentq":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра q! / if\\";
                        if (!(int.TryParse(keyWords[i + 1], out currentq) && currentq >= 0))
                            return "Error: Значением параметра q должно быть целое положительное число или 0! / if\\";
                        if (currentq > qcount)
                            return "Error: Значение параметра q должно быть меньше количества состояний! / if\\";

                        ++i;
                        break;
                    case "v":
                    case "value":
                    case "currentvalue":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра value! / if\\";
                        if (keyWords[i + 1].Length != dimensions - 1)
                            return "Error: Количество символов в value должно быть равно количеству измерений минус 1! / if\\";

                        currentvalue = keyWords[i + 1];
                        ++i;
                        break;
                }
            }
            if (currentvalue.Length == 0) return "Error: Необходимо указать value! / if\\";

            for (int i = thenIndex + 1; i < keyWords.Length; ++i)
            {
                switch (keyWords[i])
                {
                    case "q":
                    case "nextq":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра q! / then\\";
                        if (!(int.TryParse(keyWords[i + 1], out nextq) && nextq >= 0))
                            return "Error: Значением параметра q должно быть целое положительное число или 0! / then\\";
                        if (nextq > qcount)
                            return "Error: Значение параметра q должно быть меньше количества состояний! / then\\";

                        ++i;
                        break;
                    case "v":
                    case "value":
                    case "nextvalue":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра value! / then\\";
                        if (keyWords[i + 1].Length != dimensions - 1)
                            return "Error: Количество символов в value должно быть равно количеству измерений минус 1! / then\\";

                        nextvalue = keyWords[i + 1];
                        ++i;
                        break;
                    case "r":
                    case "right":
                    case "moveright":
                        move = Moves.Right;
                        break;
                    case "l":
                    case "left":
                    case "moveleft":
                        move = Moves.Left;
                        break;
                    case "s":
                    case "stop":
                        move = Moves.Stop;
                        break;
                }
            }
            if (nextvalue.Length == 0) return "Error: Необходимо указать value! / then\\";

            return turingMachine.addCommand(currentq, currentvalue, nextq, nextvalue, move);
        }

        private string add(string[] keyWords)
        {
            if (turingMachine == null) return "Error: Необходимо сначала инициализировать машину! / add\\";

            string value = string.Empty;
            string result = string.Empty;
            int index = -1;

            for (int i = 1; i < keyWords.Length; ++i)
            {
                switch (keyWords[i])
                {
                    case "v":
                    case "value":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра value! / add\\";
                        if (keyWords[i + 1].Length != dimensions - 1)
                            return "Error: Количество символов в value должно быть равно количеству измерений минус 1! / add\\";

                        value = keyWords[i + 1];
                        ++i;
                        break;
                    case "vs":
                    case "values":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра values! / add\\";

                        ++i;
                        while (i < keyWords.Length && !keyWords[i].Equals("end"))
                        {
                            if (keyWords[i].Length != dimensions - 1)
                                return "Error: Количество символов в каждом значении values должно быть равно количеству измерений минус 1! / add\\";

                            string valueString = turingMachine.addCell(keyWords[i]);

                            if (valueString.StartsWith("Error")) return valueString;
                            else result += valueString + " / ";

                            ++i;
                        }
                        break;
                    case "i":
                    case "index":
                        if (i + 1 >= keyWords.Length)
                            return "Error: Не указано значение параметра index! / add\\";
                        if (!(int.TryParse(keyWords[i + 1], out index) && index >= 0))
                            return "Error: Значением параметра index должно быть целое положительное число или 0! / add\\";

                        ++i;
                        break;
                }
            }

            if (value.Length > 0)
            {
                string valueString;

                if (index == -1) valueString = turingMachine.addCell(value);
                else valueString = turingMachine.addCell(index, value);

                if (valueString.StartsWith("Error")) return valueString;
                else result += valueString + " / ";
            }

            if (result.Length == 0) return "Error: Необходимо указать хотя бы одно value! / add\\";
            else return "Значения: / " + result + "добавлены на ленту успешно!\\";
        }

        private string print(string[] keyWords)
        {
            if (turingMachine == null) return "Error: Необходимо сначала инициализировать машину! / print\\";

            string result = "Описание:\\Измерений: " + dimensions + " / Состояний: " + qcount +
                " / Текущее состояние: " + q + "\\Временное ограничение (в миллисекундах): " + timelimit +
                "\\Ограничение по памяти (в мегабайтах): " + memorylimit + '\\';

            for (int i = 1; i < keyWords.Length; ++i)
            {
                switch (keyWords[i])
                {
                    case "c":
                    case "commands":
                        result += turingMachine.printCommands();
                        break;
                    case "t":
                    case "tape":
                        result += turingMachine.printTape();
                        break;
                }
            }
            result += "Конец описания!\\";

            return result;
        }

        public string compile(string code)
        {
            string result = string.Empty;
            string[] commands = StringHelpers.singlelineFilter(code).Split(';');

            foreach (var command in commands)
            {
                string[] keyWords = StringHelpers.cleaningFilter(command).Split(' ');
                string currentMessage;

                switch (keyWords[0])
                {
                    case "init":
                        if ((currentMessage = init(keyWords)).StartsWith("Error"))
                            return result + commandsCounter + ") " + currentMessage;
                        else result += commandsCounter + ") " + currentMessage;
                        break;
                    case "clear":
                        result += clear();
                        commandsCounter = 0;
                        break;
                    case "if":
                        if ((currentMessage = ifThen(keyWords)).StartsWith("Error"))
                            return result + commandsCounter + ") " + currentMessage;
                        else result += commandsCounter + ") " + currentMessage;
                        break;
                    case "add":
                        if ((currentMessage = add(keyWords)).StartsWith("Error"))
                            return result + commandsCounter + ") " + currentMessage;
                        else result += commandsCounter + ") " + currentMessage;
                        break;
                    case "print":
                        if ((currentMessage = print(keyWords)).StartsWith("Error"))
                            return result + commandsCounter + ") " + currentMessage;
                        else result += commandsCounter + ") " + currentMessage;
                        break;
                    case "run":
                        if ((currentMessage = turingMachine.run()).StartsWith("Error"))
                            return result + commandsCounter + ") " + currentMessage;
                        else result += commandsCounter + ") " + currentMessage;
                        break;
                }

                ++commandsCounter;
            }

            return result;
        }
    }
}