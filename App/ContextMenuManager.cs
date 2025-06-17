using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager
{
    public class MainForm : Form
    {
        private const string MenuKeyBase = @"Directory\Background\shell\";
        private CheckBox cmdCheckBox;
        private CheckBox psCheckBox;
        private CheckBox wslCheckBox;
        private CheckBox newFileCheckBox;
        private Button saveButton;

        public MainForm()
        {
            InitializeComponents();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            this.Text = "Менеджер контекстного меню";
            this.Width = 400;
            this.Height = 300;

            cmdCheckBox = new CheckBox { Text = "CMD здесь", Top = 20, Left = 20, Width = 200 };
            psCheckBox = new CheckBox { Text = "PowerShell здесь", Top = 50, Left = 20, Width = 200 };
            wslCheckBox = new CheckBox { Text = "WSL здесь", Top = 80, Left = 20, Width = 200 };
            newFileCheckBox = new CheckBox { Text = "Создать файл", Top = 110, Left = 20, Width = 200 };

            saveButton = new Button { Text = "Сохранить", Top = 150, Left = 20, Width = 100 };
            saveButton.Click += saveButton_Click;

            this.Controls.Add(cmdCheckBox);
            this.Controls.Add(psCheckBox);
            this.Controls.Add(wslCheckBox);
            this.Controls.Add(newFileCheckBox);
            this.Controls.Add(saveButton);
        }

        private void LoadSettings()
        {
            cmdCheckBox.Checked = IsContextMenuEnabled("OpenCMD");
            psCheckBox.Checked = IsContextMenuEnabled("OpenPS");
            wslCheckBox.Checked = IsContextMenuEnabled("OpenWSL");
            newFileCheckBox.Checked = IsContextMenuEnabled("NewFile");
        }

        private bool IsContextMenuEnabled(string name)
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(MenuKeyBase + name))
            {
                return key != null;
            }
        }

        private void SaveSettings()
        {
            ToggleContextMenu("OpenCMD", cmdCheckBox.Checked, "Открыть CMD здесь", "\"" + Application.ExecutablePath + "\" \"%V\" cmd");
            ToggleContextMenu("OpenPS", psCheckBox.Checked, "Открыть PowerShell здесь", "\"" + Application.ExecutablePath + "\" \"%V\" ps");
            ToggleContextMenu("OpenWSL", wslCheckBox.Checked, "Открыть WSL здесь", "\"" + Application.ExecutablePath + "\" \"%V\" wsl");
            ToggleContextMenu("NewFile", newFileCheckBox.Checked, "Создать файл...", "\"" + Application.ExecutablePath + "\" \"%V\" newfile");
        }

        private void ToggleContextMenu(string name, bool enabled, string menuText, string command)
        {
            string keyPath = MenuKeyBase + name;
            string cmdPath = keyPath + "\\command";

            if (enabled)
            {
                using (var key = Registry.ClassesRoot.CreateSubKey(keyPath))
                {
                    key.SetValue("", menuText);
                    key.SetValue("Icon", "shell32.dll,-50");
                }

                using (var cmdKey = Registry.ClassesRoot.CreateSubKey(cmdPath))
                {
                    cmdKey.SetValue("", command);
                }
            }
            else
            {
                Registry.ClassesRoot.DeleteSubKeyTree(keyPath, false);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Настройки сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void HandleCommand(string folderPath, string action)
        {
            switch (action.ToLower())
            {
                case "cmd":
                    Process.Start("cmd.exe", "/K \"cd /d \"" + folderPath + "\"");
                    break;
                case "ps":
                    Process.Start("powershell.exe", "-NoExit -Command \"Set-Location -Path '" + folderPath.Replace("'", "''") + "'\"");
                    break;
                case "wsl":
                    Process.Start("wsl.exe", "--cd \"" + folderPath.Replace("\\", "/").Replace("C:", "/mnt/c") + "\"");
                    break;
                case "newfile":
                    var inputForm = new Form { Text = "Создать файл", Width = 300, Height = 150 };
                    var textBox = new TextBox { Top = 20, Left = 20, Width = 240 };
                    var createButton = new Button { Text = "Создать", Top = 60, Left = 20, Width = 100 };
                    
                    createButton.Click += (s, ev) => {
                        try
                        {
                            File.Create(Path.Combine(folderPath, textBox.Text)).Close();
                            inputForm.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при создании файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    
                    inputForm.Controls.Add(textBox);
                    inputForm.Controls.Add(createButton);
                    inputForm.ShowDialog();
                    break;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                HandleCommand(args[0], args[1]);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
