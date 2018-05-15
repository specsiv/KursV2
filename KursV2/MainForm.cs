using KursV2.Logic;
using KursV2.Helpers;
using System;
using System.Windows.Forms;
using KursV2.Interfaces;

namespace KursV2
{
    public partial class MainForm : Form
    {
        private ICommandsManager commandsManager;
        private string savePath = string.Empty;

        private void run()
        {
            string[] response = commandsManager.compile(inputField.Text).Split('\\');

            for (int i = 0; i < response.Length - 1; ++i)
            {
                if (response[i].Equals("Отчистка прошла успешно!"))
                {
                    outputField.Text = string.Empty;
                }

                outputField.Text += ">" + response[i] + "\r";
            }
        }

        private void load(string path, bool needToRun = false)
        {
            string code;
            if ((code = StringHelpers.load(path)) != null)
            {
                inputField.Text = code;
                if (needToRun) run();
            }
            else outputField.Text = ">Error: Произошла ошибка при попытке загрузки файла! / load\r";
        }

        private bool save(string path)
        {
            if (!StringHelpers.save(path, inputField.Text))
            {
                outputField.Text += ">Error: Произошла ошибка при попытке сохранения файла! / save\r";

                return false;
            }

            return true;
        }

        public MainForm()
        {
            InitializeComponent();

            commandsManager = new CommandsManager();
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                outputField.Text += ">Открыто: " + args[1] + '\r';

                load(args[1], true);

                savePath = args[1];
                Text = StringHelpers.simpleSplit(args[1], '\\', true)[1];
            }
        }

        private void sendCmdBtn_Click(object sender, EventArgs e)
        {
            run();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                load(openFileDialog.FileName);

                outputField.Text += ">Открыто: " + openFileDialog.FileName + '\r';
                savePath = openFileDialog.FileName;
                Text = openFileDialog.SafeFileName;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                if (save(saveFileDialog.FileName))
                    Text = StringHelpers.simpleSplit(saveFileDialog.FileName, '\\', true)[1];

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savePath.Length == 0)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    if (save(saveFileDialog.FileName))
                    {
                        savePath = saveFileDialog.FileName;
                        Text = StringHelpers.simpleSplit(savePath, '\\', true)[1];
                    }
            }
            else
            {
                save(savePath);
                Text = Text.TrimEnd('*');
            }
        }

        private void inputField_TextChanged(object sender, EventArgs e)
        {
            if (Text[Text.Length - 1] != '*') Text += "*";
        }
    }
}