using MazeGen.Data;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class PlayerMainForm : Form
    {
        private Database db;
        private MazeService mazeService;
        private Theme currentTheme = Theme.Forest;
        private Panel pnlMazeView;

        // Состояние прохождения
        private Maze currentMaze;
        private List<Point> currentPath = new List<Point>();
        private int currentPathIndex = 0;
        private bool isPlaying = false;
        private System.Windows.Forms.Timer gameTimer;

        private string currentAlgorithm = ""; // "wave" or "righthand"

        // Изображения темы
        private Dictionary<string, Bitmap> cellImages = new();
        
        public PlayerMainForm(Database db, MazeService ms)
        {
            this.db = db;
            mazeService = ms;
            InitializeComponent();
            LoadThemeImages();
        }

        private void InitializeComponent()
        {
            Text = $"MazeGen - Игрок";
            Size = new Size(1200, 720);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            // Меню
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("Файл");

            var loadMazeItem = new ToolStripMenuItem("Загрузить лабиринт");
            loadMazeItem.Click += (s, e) => LoadMaze();
            fileMenu.DropDownItems.Add(loadMazeItem);

            var exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => Close();
            fileMenu.DropDownItems.Add(exitItem);
            
            var authItem = new ToolStripMenuItem("Авторизация");
            authItem.Click += (s, e) => Close();
            fileMenu.DropDownItems.Insert(0, authItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            
            var aboutItem = new ToolStripMenuItem("О разработчиках");
            aboutItem.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(aboutItem);

            var systemItem = new ToolStripMenuItem("О системе");
            systemItem.Click += (s, e) => ShowSystemInfo();
            helpMenu.DropDownItems.Add(systemItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Левая панель с параметрами
            var pnlLeft = new Panel
            {
                Location = new Point(10, 30),
                Size = new Size(350, 650),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // Блок выбора темы
            var themeGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 15),
                Size = new Size(320, 85)
            };

            var lblTheme = new Label
            {
                Text = "Тема оформления",
                Location = new Point(10, 0),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblTheme);

            string[] themes = { "Лес", "Город", "Пустыня", "Подземелье" };
            Theme[] themeValues = { Theme.Forest, Theme.City, Theme.Desert, Theme.Dungeon };
            for (int i = 0; i < themes.Length; i++)
            {
                var rbLeft = new RadioButton
                {
                    Text = themes[i],
                    Location = new Point(20, 15 + i * 15),
                    Checked = (i == 0),
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true
                };
                int idxLeft = i;
                rbLeft.CheckedChanged += (s, e) => {
                    if ((s as RadioButton).Checked) {
                        currentTheme = themeValues[idxLeft];
                        LoadThemeImages();
                        pnlMazeView.Invalidate();
                    }
                };
                themeGroup.Controls.Add(rbLeft);
                i++;

                var rbRight = new RadioButton
                {
                    Text = themes[i],
                    Location = new Point(150, 15 + (i - 1) * 15),
                    Checked = (i == 0),
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true
                };
                int idxRight = i;
                rbRight.CheckedChanged += (s, e) => {
                    if ((s as RadioButton).Checked) {
                        currentTheme = themeValues[idxRight];
                        LoadThemeImages();
                        pnlMazeView.Invalidate();
                    }
                };
                themeGroup.Controls.Add(rbRight);
            }
            pnlLeft.Controls.Add(themeGroup);

            // Блок режима прохождения
            var lblPlacement = new Label
            {
                Text = "Режим прохождения",
                Location = new Point(10, 110),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblPlacement);

            var placementGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 125),
                Size = new Size(320, 50)
            };
            var rbAuto = new RadioButton
            {
                Text = "Авто",
                Location = new Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbAuto",
                Font = new Font("Segoe UI", 12)
            };

            var rbManual = new RadioButton
            {
                Text = "Вручную",
                Location = new Point(200, 15),
                AutoSize = true,
                Name = "rbManual",
                Font = new Font("Segoe UI", 12)
            };
            placementGroup.Controls.Add(rbAuto);
            placementGroup.Controls.Add(rbManual);
            pnlLeft.Controls.Add(placementGroup);

            var btnApply = new Button
            {
                Text = "Применить",
                Location = new Point(70, 190),
                Size = new Size(200, 35),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnApply"
            };
            btnApply.Click += (s, e) => ApplyMode();
            pnlLeft.Controls.Add(btnApply);

            var btnStart = new Button
            {
                Text = "Начать прохождение",
                Location = new Point(70, 235),
                Size = new Size(200, 35),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnStart",
                Enabled = false
            };
            btnStart.Click += (s, e) => StartGame();
            pnlLeft.Controls.Add(btnStart);

            var btnMakeStep = new Button
            {
                Text = "Сделать шаг",
                Location = new Point(70, 280),
                Size = new Size(200, 35),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnMakeStep",
                Visible = false,
                Enabled = false
            };
            btnMakeStep.Click += (s, e) => MakeStep();
            pnlLeft.Controls.Add(btnMakeStep);

            // Блок выбора алгоритма прохождения
            var lblAlgorithm = new Label
            {
                Text = "Алгоритм прохождения",
                Location = new Point(10, 320),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12),
                Name = "lblAlgorithm",
                Visible = false
            };
            pnlLeft.Controls.Add(lblAlgorithm);

            var algorithmGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 335),
                Size = new Size(320, 50),
                Name = "algorithmGroup",
                Visible = false
            };

            var rbWave = new RadioButton
            {
                Text = "Волновой",
                Location = new Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbWave",
                Font = new Font("Segoe UI", 12)
            };

            var rbHand = new RadioButton
            {
                Text = "Правой руки",
                Location = new Point(190, 15),
                AutoSize = true,
                Name = "rbHand",
                Font = new Font("Segoe UI", 12)
            };
            algorithmGroup.Controls.Add(rbWave);
            algorithmGroup.Controls.Add(rbHand);
            pnlLeft.Controls.Add(algorithmGroup);

            // Блок выбора режима автоматического прохождения
            var lblMode = new Label
            {
                Text = "Режим автоматического прохождения",
                Location = new Point(10, 400),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 12),
                Name = "lblMode",
                Visible = false
            };
            pnlLeft.Controls.Add(lblMode);

            var modeGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 415),
                Size = new Size(320, 50),
                Name = "modeGroup",
                Visible = false
            };

            var rbStep = new RadioButton
            {
                Text = "Пошаговый",
                Location = new Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbStep",
                Font = new Font("Segoe UI", 12)
            };
            rbStep.CheckedChanged += (s, e) =>
            {
                var lblSpeed = Controls.Find("lblSpeed", true)[0];
                var speedGroup = Controls.Find("speedGroup", true)[0];
                if (rbStep.Checked)
                {
                    lblSpeed.Visible = false;
                    speedGroup.Visible = false;
                }
            };

            var rbDelay = new RadioButton
            {
                Text = "С задержкой",
                Location = new Point(190, 15),
                AutoSize = true,
                Name = "rbDelay",
                Font = new Font("Segoe UI", 12)
            };
            rbDelay.CheckedChanged += (s, e) =>
            {
                var lblSpeed = Controls.Find("lblSpeed", true)[0];
                var speedGroup = Controls.Find("speedGroup", true)[0];
                lblSpeed.Visible = rbDelay.Checked;
                speedGroup.Visible = rbDelay.Checked;
            };
            modeGroup.Controls.Add(rbStep);
            modeGroup.Controls.Add(rbDelay);
            pnlLeft.Controls.Add(modeGroup);

            // Блок выбора скорости прохождения (видимый только для режима с задержкой)
            var lblSpeed = new Label
            {
                Text = "Скорость прохождения",
                Location = new Point(10, 480),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12),
                Name = "lblSpeed",
                Visible = false
            };
            pnlLeft.Controls.Add(lblSpeed);

            var speedGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 500),
                Size = new Size(320, 80),
                Name = "speedGroup",
                Visible = false
            };

            var tbSpeed = new TrackBar
            {
                Location = new Point(10, 10),
                Minimum = 1,
                Maximum = 3,
                Value = 2,
                TickFrequency = 1,
                Name = "tbSpeed",
                Size = new Size(280, 45)
            };
            speedGroup.Controls.Add(tbSpeed);

            var lblSpeedLow = new Label
            {
                Text = "Низкая",
                Location = new Point(10, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            var lblSpeedMid = new Label
            {
                Text = "Средняя",
                Location = new Point(125, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            var lblSpeedHigh = new Label
            {
                Text = "Высокая",
                Location = new Point(250, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            speedGroup.Controls.Add(lblSpeedLow);
            speedGroup.Controls.Add(lblSpeedMid);
            speedGroup.Controls.Add(lblSpeedHigh);

            var regularFont = new Font("Segoe UI", 9, FontStyle.Regular);
            var boldFont = new Font("Segoe UI", 9, FontStyle.Bold);

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

            Controls.Add(pnlLeft);

            // Правая панель для визуализации
            pnlMazeView = new OptimizedPanel
            {
                Location = new Point(370, 30),
                Size = new Size(810, 650),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "pnlMazeView",
                BackColor = Color.White
            };
            pnlMazeView.Paint += PnlMazeView_Paint;
            pnlMazeView.MouseClick += PnlMazeView_MouseClick;
            Controls.Add(pnlMazeView);

            // Инициализация таймера для автоматического прохождения
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Tick += GameTimer_Tick;
        }

        private void PnlMazeView_Paint(object sender, PaintEventArgs e)
        {
            if (currentMaze == null) return;
            var g = e.Graphics;
            int w = pnlMazeView.Width / currentMaze.Width;
            int h = pnlMazeView.Height / currentMaze.Height;

            // Местность
            for (int x = 0; x < currentMaze.Width; x++)
            {
                for (int y = 0; y < currentMaze.Height; y++)
                {
                    Rectangle cellRect = new Rectangle(x * w, y * h, w, h);
                    Bitmap img = null;

                    if (currentMaze.Entrance == new Point(x, y))
                        img = cellImages["entrance"];
                    else if (currentMaze.Exit == new Point(x, y))
                        img = cellImages["exit"];
                    else if (currentMaze.Grid[x, y] == 1)
                        img = cellImages["path"];
                    else
                        img = cellImages["wall"];

                    g.DrawImage(img, cellRect);
                }
            }

            // Пройденный путь
            if (currentPathIndex > 1 && currentPath.Count > 0)
            {
                for (int i = 0; i < currentPathIndex - 1; i++)
                {
                    var pt = currentPath[i];
                    if (pt != currentMaze.Entrance && pt != currentMaze.Exit)
                    {
                        Rectangle cellRect = new Rectangle(pt.X * w, pt.Y * h, w, h);
                        g.DrawImage(cellImages["passed"], cellRect);
                    }
                }
            }

            // Положение персонажа
            if (currentPathIndex > 0 && currentPath.Count > currentPathIndex - 1)
            {
                var pos = currentPath[currentPathIndex - 1];
                Rectangle cellRect = new Rectangle(pos.X * w, pos.Y * h, w, h);
                g.DrawImage(cellImages["player"], cellRect);
            }
        }

        private void PnlMazeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentMaze == null || !isPlaying) return;

            var rbManual = Controls.Find("rbManual", true)[0] as RadioButton;
            if (!rbManual.Checked) return;

            int cellW = pnlMazeView.Width / currentMaze.Width;
            int cellH = pnlMazeView.Height / currentMaze.Height;
            int x = e.X / cellW;
            int y = e.Y / cellH;
            var pt = new Point(x, y);

            // Текущая позиция игрока
            var currentPos = currentPath.Last();

            // Проверяем, что клик по проходу и на расстоянии 1
            if (currentMaze.Grid[x, y] == 1 &&
                Math.Abs(currentPos.X - x) + Math.Abs(currentPos.Y - y) == 1)
            {
                currentPath.Add(pt);
                currentPathIndex = currentPath.Count;

                if (pt == currentMaze.Exit)
                {
                    pnlMazeView.Invalidate();
                    MessageBox.Show("Вы прошли лабиринт!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isPlaying = false;
                    return;
                }
                pnlMazeView.Invalidate();
            }
        }

        private void LoadMaze()
        {
            var form = new LoadMazeForm(db, this);
            form.ShowDialog(this);
        }

        private void ApplyMode()
        {
            if (currentMaze == null)
            {
                MessageBox.Show("Сначала загрузите лабиринт.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rbAuto = Controls.Find("rbAuto", true)[0] as RadioButton;
            var lblAlgorithm = Controls.Find("lblAlgorithm", true)[0];
            var algorithmGroup = Controls.Find("algorithmGroup", true)[0];
            var lblMode = Controls.Find("lblMode", true)[0];
            var modeGroup = Controls.Find("modeGroup", true)[0];
            var lblSpeed = Controls.Find("lblSpeed", true)[0];
            var speedGroup = Controls.Find("speedGroup", true)[0];
            var btnStart = Controls.Find("btnStart", true)[0];

            if (rbAuto.Checked)
            {
                lblAlgorithm.Visible = true;
                algorithmGroup.Visible = true;
                lblMode.Visible = true;
                modeGroup.Visible = true;
            }
            else
            {
                lblAlgorithm.Visible = false;
                algorithmGroup.Visible = false;
                lblMode.Visible = false;
                modeGroup.Visible = false;
                lblSpeed.Visible = false;
                speedGroup.Visible = false;
            }

            btnStart.Enabled = true;
        }

        private void StartGame()
        {
            if (currentMaze == null) return;

            var rbAuto = Controls.Find("rbAuto", true)[0] as RadioButton;
            var rbManual = Controls.Find("rbManual", true)[0] as RadioButton;

            currentPathIndex = 1;
            currentPath = new List<Point> { currentMaze.Entrance };
            isPlaying = true;

            if (rbManual.Checked)
            {
                MessageBox.Show("Нажимайте на соседние клетки для движения от входа к выходу.", "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pnlMazeView.Invalidate();
            }
            else if (rbAuto.Checked)
            {
                var rbWave = Controls.Find("rbWave", true)[0] as RadioButton;
                string algorithm = rbWave.Checked ? "wave" : "righthand";
                currentAlgorithm = algorithm;

                if (algorithm == "wave")
                    currentPath = mazeService.FindPathWave(currentMaze);
                else
                    currentPath = mazeService.FindPathRightHand(currentMaze);

                if (currentPath.Count == 0)
                {
                    MessageBox.Show("Путь не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isPlaying = false;
                    return;
                }

                var rbStep = Controls.Find("rbStep", true)[0] as RadioButton;
                var btnMakeStep = Controls.Find("btnMakeStep", true)[0];
                
                if (rbStep.Checked)
                {
                    btnMakeStep.Visible = true;
                    btnMakeStep.Enabled = true;
                    pnlMazeView.Invalidate();
                }
                else
                {
                    var tbSpeed = Controls.Find("tbSpeed", true)[0] as TrackBar;
                    int speed = tbSpeed.Value;
                    int interval = speed == 1 ? 500 : (speed == 2 ? 250 : 100);
                    gameTimer.Interval = interval;
                    gameTimer.Start();
                    pnlMazeView.Invalidate();
                }
            }
        }

        private void MakeStep()
        {
            if (currentPath.Count <= currentPathIndex) return;
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count || currentPath[currentPathIndex - 1] == currentMaze.Exit)
            {
                MessageBox.Show("Вы прошли лабиринт!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isPlaying = false;
                Controls.Find("btnMakeStep", true)[0].Visible = false;
            }

            pnlMazeView.Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (currentPath.Count <= currentPathIndex || !isPlaying)
            {
                pnlMazeView.Invalidate();
                gameTimer.Stop();
                MessageBox.Show("Вы прошли лабиринт!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isPlaying = false;
                return;
            }
            currentPathIndex++;

            // Удаление тупиков из пути
            if (currentAlgorithm == "righthand")
            {
                int posIdx = currentPathIndex - 1;
                if (posIdx >= 0 && posIdx < currentPath.Count)
                {
                    var pos = currentPath[posIdx];
                    int earlierIndex = currentPath.FindIndex(p => p == pos);
                    if (earlierIndex != -1 && earlierIndex < posIdx)
                    {
                        int removeStart = earlierIndex + 1;
                        int removeCount = posIdx - earlierIndex;
                        if (removeCount > 0 && removeStart >= 0 && removeStart + removeCount <= currentPath.Count)
                        {
                            currentPath.RemoveRange(removeStart, removeCount);
                            currentPathIndex = earlierIndex + 1;
                        }
                    }
                }
            }

            pnlMazeView.Invalidate();
        }

        private void ShowAbout()
        {
            var form = new AboutForm();
            form.ShowDialog(this);
        }
        private void ShowSystemInfo()
        {
            // Открытие HTML-страницы с описанием функций игрока
            string helpFile = "C:\\Users\\User\\source\\repos\\MazeGen\\Help.html";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = helpFile,
                UseShellExecute = true
            });
        }

        // Отображение лабиринта
        public void LoadMazeFromForm(Maze maze)
        {
            currentMaze = maze;
            currentPathIndex = 0;
            currentPath.Clear();
            pnlMazeView.Invalidate();
        }
        
        // Загрузка темы
        private void LoadThemeImages()
        {
            string themeName = currentTheme.ToString().ToLower();
            string basePath = "C:\\Users\\User\\source\\repos\\MazeGen\\Resources\\Themes";

            cellImages["wall"] = new Bitmap(Path.Combine(basePath, $"{themeName}_wall.png"));
            cellImages["path"] = new Bitmap(Path.Combine(basePath, $"{themeName}_path.png"));
            cellImages["entrance"] = new Bitmap(Path.Combine(basePath, $"{themeName}_entrance.png"));
            cellImages["exit"] = new Bitmap(Path.Combine(basePath, $"{themeName}_exit.png"));
            cellImages["passed"] = new Bitmap(Path.Combine(basePath, $"{themeName}_passed.png"));
            cellImages["player"] = new Bitmap(Path.Combine(basePath, $"{themeName}_player.png"));
        }
    }
}