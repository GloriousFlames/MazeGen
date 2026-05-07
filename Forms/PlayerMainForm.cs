using System;
using System.Windows.Forms;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class PlayerMainForm : Form
    {
        private User currentUser;
        private MazeService mazeService;
        private Theme currentTheme = Theme.Forest;

        public PlayerMainForm(User user)
        {
            currentUser = user;
            mazeService = new MazeService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"MazeGen - Игрок ({currentUser.Login})";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Меню
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("Файл");
            var loadMazeItem = new ToolStripMenuItem("Загрузить лабиринт");
            loadMazeItem.Click += (s, e) => LoadMaze();
            fileMenu.DropDownItems.Add(loadMazeItem);
            var exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => this.Close();
            fileMenu.DropDownItems.Add(exitItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            var aboutItem = new ToolStripMenuItem("О разработчиках");
            aboutItem.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);


            // Левая панель с параметрами
            var pnlLeft = new Panel
            {
                Location = new System.Drawing.Point(10, 30),
                Size = new System.Drawing.Size(350, 650),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // Блок выбора темы
            var themeGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 15),
                Size = new System.Drawing.Size(320, 85)
            };

            var lblTheme = new Label
            {
                Text = "Тема оформления",
                Location = new System.Drawing.Point(10, 0),
                Size = new System.Drawing.Size(150, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblTheme);

            string[] themes = { "Лес", "Горы", "Пустыня", "Равнина" };
            Theme[] themeValues = { Theme.Forest, Theme.Mountains, Theme.Desert, Theme.Plain };
            for (int i = 0; i < themes.Length; i++)
            {
                var rbLeft = new RadioButton
                {
                    Text = themes[i],
                    Location = new Point(20, 15 + i * 15),
                    Checked = (i == 0),
                    Font = new System.Drawing.Font("Segoe UI", 12),
                    AutoSize = true
                };
                int idxLeft = i;
                rbLeft.CheckedChanged += (s, e) => { if ((s as RadioButton).Checked) currentTheme = themeValues[idxLeft]; };
                themeGroup.Controls.Add(rbLeft);
                i++;

                var rbRight = new RadioButton
                {
                    Text = themes[i],
                    Location = new Point(150, 15 + (i - 1) * 15),
                    Checked = (i == 0),
                    Font = new System.Drawing.Font("Segoe UI", 12),
                    AutoSize = true
                };
                int idxRight = i;
                rbRight.CheckedChanged += (s, e) => { if ((s as RadioButton).Checked) currentTheme = themeValues[idxRight]; };
                themeGroup.Controls.Add(rbRight);
            }
            pnlLeft.Controls.Add(themeGroup);

            // Блок режима прохождения
            var lblPlacement = new Label
            {
                Text = "Режим прохождения",
                Location = new System.Drawing.Point(10, 110),
                Size = new System.Drawing.Size(250, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblPlacement);

            var placementGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 125),
                Size = new System.Drawing.Size(320, 50)
            };
            var rbAuto = new RadioButton
            {
                Text = "Авто",
                Location = new System.Drawing.Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbAuto",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };

            var rbManual = new RadioButton
            {
                Text = "Вручную",
                Location = new System.Drawing.Point(200, 15),
                AutoSize = true,
                Name = "rbManual",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            placementGroup.Controls.Add(rbAuto);
            placementGroup.Controls.Add(rbManual);
            pnlLeft.Controls.Add(placementGroup);

            var btnApply = new Button
            {
                Text = "Применить",
                Location = new System.Drawing.Point(70, 190),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnApply.Click += (s, e) => ApplyMode();
            pnlLeft.Controls.Add(btnApply);

            var btnStart = new Button
            {
                Text = "Начать прохождение",
                Location = new System.Drawing.Point(70, 235),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnStart.Click += (s, e) => ApplyMode();
            pnlLeft.Controls.Add(btnStart);

            var btnMakeStep = new Button
            {
                Text = "Сделать шаг",
                Location = new System.Drawing.Point(70, 280),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnMakeStep.Click += (s, e) => MakeStep();
            pnlLeft.Controls.Add(btnMakeStep);

            // Блок выбора алгоритма прохождения
            var lblAlgorithm = new Label
            {
                Text = "Алгоритм прохождеиня",
                Location = new System.Drawing.Point(10, 320),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblAlgorithm);

            var algorithmGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 335),
                Size = new System.Drawing.Size(320, 50)
            };

            var rbWave = new RadioButton
            {
                Text = "Волновой",
                Location = new System.Drawing.Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbWave",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };

            var rbHand = new RadioButton
            {
                Text = "Правой руки",
                Location = new System.Drawing.Point(190, 15),
                AutoSize = true,
                Name = "rbHand",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            algorithmGroup.Controls.Add(rbWave);
            algorithmGroup.Controls.Add(rbHand);
            pnlLeft.Controls.Add(algorithmGroup);

            // Блок выбора алгоритма прохождения
            var lblMode = new Label
            {
                Text = "Режим автоматического прохождения",
                Location = new System.Drawing.Point(10, 400),
                Size = new System.Drawing.Size(250, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblMode);

            var modeGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 415),
                Size = new System.Drawing.Size(320, 50)
            };

            var rbStep = new RadioButton
            {
                Text = "Пошаговый",
                Location = new System.Drawing.Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbStep",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };

            var rbDelay = new RadioButton
            {
                Text = "С задержкой",
                Location = new System.Drawing.Point(190, 15),
                AutoSize = true,
                Name = "rbDelay",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            modeGroup.Controls.Add(rbStep);
            modeGroup.Controls.Add(rbDelay);
            pnlLeft.Controls.Add(modeGroup);

            // Блок выбора скорости прохождения
            var lblSpeed = new Label
            {
                Text = "Скорость прохождения",
                Location = new System.Drawing.Point(10, 480),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblSpeed);

            var speedGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 500),
                Size = new System.Drawing.Size(320, 80)
            };

            var tbSpeed = new TrackBar
            {
                Location = new System.Drawing.Point(10, 10),
                Minimum = 1,
                Maximum = 3,
                Value = 2,
                TickFrequency = 1,
                Name = "tbSpeed",
                Size = new System.Drawing.Size(280, 45)
            };
            speedGroup.Controls.Add(tbSpeed);

            var lblSpeedLow = new Label
            {
                Text = "Низкая",
                Location = new System.Drawing.Point(10, 55),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            var lblSpeedMid = new Label
            {
                Text = "Средняя",
                Location = new System.Drawing.Point(125, 55),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            var lblSpeedHigh = new Label
            {
                Text = "Высокая",
                Location = new System.Drawing.Point(250, 55),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            speedGroup.Controls.Add(lblSpeedLow);
            speedGroup.Controls.Add(lblSpeedMid);
            speedGroup.Controls.Add(lblSpeedHigh);

            var regularFont = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular);
            var boldFont = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            Action updateLabels = () =>
            {
                lblSpeedLow.Font = regularFont;
                lblSpeedMid.Font = regularFont;
                lblSpeedHigh.Font = regularFont;
                switch (tbSpeed.Value)
                {
                    case 1: lblSpeedLow.Font = boldFont; break;
                    case 2: lblSpeedMid.Font = boldFont; break;
                    case 3: lblSpeedHigh.Font = boldFont; break;
                }
            };
            tbSpeed.ValueChanged += (s, e) => updateLabels();
            updateLabels();
            pnlLeft.Controls.Add(speedGroup);

            this.Controls.Add(pnlLeft);

            // Правая панель для визуализации
            var pnlRight = new Panel
            {
                Location = new System.Drawing.Point(370, 30),
                Size = new System.Drawing.Size(810, 650),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "pnlMazeView",
                BackColor = System.Drawing.Color.White
            };
            this.Controls.Add(pnlRight);
        }

        private void LoadMaze()
        {
            var form = new LoadMazeForm();
            form.ShowDialog(this);
        }

        private void ApplyMode()
        {
            MessageBox.Show("Режим применен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StartGame()
        {
            MessageBox.Show("Игра началась!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void MakeStep()
        {
            MessageBox.Show("Шаг сделан", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            var form = new AboutForm();
            form.ShowDialog(this);
        }
    }
}