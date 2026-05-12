using MazeGen.Data;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class AdminMainForm : Form
    {
        private Database db;
        private User currentUser;
        private MazeService mazeService;
        private Theme currentTheme = Theme.Forest;

        private Maze currentMaze;
        private bool isPlacingEntranceExit = false;
        private int placementStep = 0;
        private Point? manualEntrance = null;
        private Point? manualExit = null;
        private List<Point> pathCells = new List<Point>();
        private Panel pnlMazeView;

        // Изображения темы
        private Dictionary<string, Bitmap> cellImages = new();

        // Доступные клетки для входа и выхода
        private List<Point> availableEntranceExitCells = new List<Point>();

        public AdminMainForm(User user, Database db, MazeService ms)
        {
            currentUser = user;
            this.db = db;
            mazeService = ms;
            InitializeComponent();
            LoadThemeImages();
            pnlMazeView = Controls.Find("pnlMazeView", true)[0] as Panel;
            pnlMazeView.Paint += PnlMazeView_Paint;
            pnlMazeView.MouseClick += PnlMazeView_MouseClick;
        }

        private void InitializeComponent()
        {
            Text = "MazeGen - Администратор";
            Size = new Size(1200, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            // Меню
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("Файл");

            var saveMazeItem = new ToolStripMenuItem("Сохранить лабиринт");
            saveMazeItem.Click += (s, e) => SaveMaze();
            fileMenu.DropDownItems.Add(saveMazeItem);

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

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            // Левая панель
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
                    if ((s as RadioButton).Checked)
                    {
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
                    if ((s as RadioButton).Checked)
                    {
                        currentTheme = themeValues[idxRight];
                        LoadThemeImages();
                        pnlMazeView.Invalidate();
                    }
                };
                themeGroup.Controls.Add(rbRight);
            }
            pnlLeft.Controls.Add(themeGroup);

            // Блок параметров лабиринта
            var paramsGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 125),
                Size = new Size(320, 100)
            };

            var lblParams = new Label
            {
                Text = "Параметры лабиринта",
                Location = new Point(10, 110),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 12),
            };
            pnlLeft.Controls.Add(lblParams);

            var lblWidth = new Label 
            { 
                Text = "Ширина", 
                Location = new Point(120, 20), 
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 12)
            };
            paramsGroup.Controls.Add(lblWidth);

            var numWidth = new NumericUpDown 
            { 
                Name = "numWidth", 
                Value = 15, 
                Minimum = 7, 
                Maximum = 25, 
                Location = new Point(200, 20), 
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 12)
            };
            paramsGroup.Controls.Add(numWidth);
            
            var lblHeight = new Label 
            { 
                Text = "Длина", 
                Location = new Point(120, 60), 
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 12),
            };
            paramsGroup.Controls.Add(lblHeight);
            
            var numHeight = new NumericUpDown 
            { 
                Name = "numHeight", 
                Value = 15, 
                Minimum = 7, 
                Maximum = 21, 
                Location = new Point(200, 60), 
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 12),
            };
            paramsGroup.Controls.Add(numHeight);
            
            pnlLeft.Controls.Add(paramsGroup);

            var btnCreateTemplate = new Button
            {
                Text = "Создать шаблон",
                Location = new Point(70, 235),
                Size = new Size(200, 35),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnCreateTemplate"
            };
            btnCreateTemplate.Click += (s, e) => CreateTemplate();
            pnlLeft.Controls.Add(btnCreateTemplate);

            // Блок расстановки входа/выхода
            var lblPlacement = new Label
            {
                Text = "Расстановка входа/выхода:",
                Location = new Point(10, 280),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblPlacement);

            var placementGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 300),
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

            var btnApplyPlacement = new Button
            {
                Text = "Применить",
                Location = new Point(100, 360),
                Size = new Size(150, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnApplyPlacement",
                Enabled = false // изначально недоступна
            };
            btnApplyPlacement.Click += (s, e) => ApplyPlacement();
            pnlLeft.Controls.Add(btnApplyPlacement);

            // Блок выбора алгоритма
            var lblAlgorithm = new Label
            {
                Text = "Алгоритм генерации",
                Location = new Point(10, 410),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblAlgorithm);

            var algorithmGroup = new GroupBox
            {
                Text = "",
                Location = new Point(10, 420),
                Size = new Size(320, 50)
            };

            var rbPrim = new RadioButton
            {
                Text = "Прима",
                Location = new Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbPrim",
                Font = new Font("Segoe UI", 12)
            };

            var rbEller = new RadioButton
            {
                Text = "Эллера",
                Location = new Point(200, 15),
                AutoSize = true,
                Name = "rbEller",
                Font = new Font("Segoe UI", 12)
            };
            algorithmGroup.Controls.Add(rbPrim);
            algorithmGroup.Controls.Add(rbEller);
            pnlLeft.Controls.Add(algorithmGroup);

            var btnGenerateMaze = new Button
            {
                Text = "Создать лабиринт",
                Location = new Point(70, 480),
                Size = new Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Name = "btnGenerateMaze",
                Enabled = false
            };
            btnGenerateMaze.Click += (s, e) => GenerateMaze();
            pnlLeft.Controls.Add(btnGenerateMaze);
            Controls.Add(pnlLeft);

            // Правая панель для визуализации
            var pnlRight = new OptimizedPanel
            {
                Location = new Point(370, 30),
                Size = new Size(810, 640),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "pnlMazeView",
                BackColor = System.Drawing.Color.White
            };
            this.Controls.Add(pnlRight);
        }

        private void CreateTemplate()
        {   
            int width = (int)((NumericUpDown)Controls.Find("numWidth", true)[0]).Value;
            int height = (int)((NumericUpDown)Controls.Find("numHeight", true)[0]).Value;
            currentMaze = mazeService.CreateTemplate(width, height);
            manualEntrance = null;
            manualExit = null;
            pathCells.Clear();
            pnlMazeView.Invalidate();
            Controls.Find("btnApplyPlacement", true)[0].Enabled = true;
            Controls.Find("btnGenerateMaze", true)[0].Enabled = false;
        }

        // Добавление доступных клеток для входа и выхода
        private List<Point> GetAvailableEntranceExitCells()
        {
            var result = new List<Point>();
            if (currentMaze == null) return result;
            int w = currentMaze.Width, h = currentMaze.Height;
            for (int x = 1; x < w - 1; x++)
            {
                if (x % 2 == 1)
                {
                    result.Add(new Point(x, 0));
                    result.Add(new Point(x, h - 1));
                }
            }
            for (int y = 1; y < h - 1; y++)
            {
                if (y % 2 == 1)
                {
                    result.Add(new Point(0, y));
                    result.Add(new Point(w - 1, y));
                }
            }
            return result;
        }

        // Применить расстановку
        private void ApplyPlacement()
        {
            var rbAuto = Controls.Find("rbAuto", true)[0] as RadioButton;
            if (rbAuto.Checked)
            {
                mazeService.PlaceEntranceExit(currentMaze, true);
                manualEntrance = null;
                manualExit = null;
                availableEntranceExitCells.Clear();
                pnlMazeView.Invalidate();
                Controls.Find("btnGenerateMaze", true)[0].Enabled = true;
                MessageBox.Show("Вход и выход расставлены автоматически!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                isPlacingEntranceExit = true;
                placementStep = 0;
                manualEntrance = null;
                manualExit = null;
                availableEntranceExitCells = GetAvailableEntranceExitCells();
                pnlMazeView.Invalidate();
                MessageBox.Show("Нажмите на одну из подсвеченных клеток для выбора входа, а затем выхода.", "Ручная расстановка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Визуализация лабиринта
        private void PnlMazeView_Paint(object sender, PaintEventArgs e)
        {
            if (currentMaze == null) return;
            var g = e.Graphics;
            int w = pnlMazeView.Width / currentMaze.Width;
            int h = pnlMazeView.Height / currentMaze.Height;

            for (int x = 0; x < currentMaze.Width; x++)
            {
                for (int y = 0; y < currentMaze.Height; y++)
                {
                    Rectangle cellRect = new Rectangle(x * w, y * h, w, h);

                    Bitmap img = null;
                    if (!(x == 0 && y == 0) && currentMaze.Entrance == new Point(x, y))
                        img = cellImages["entrance"];
                    else if (!(x == 0 && y == 0) && currentMaze.Exit == new Point(x, y))
                        img = cellImages["exit"];
                    else if (currentMaze.Grid[x, y] == 1)
                        img = cellImages["path"];
                    else
                        img = cellImages["wall"];

                    g.DrawImage(img, cellRect);
                }
            }

            // Подсветка доступных клеток для входа/выхода при ручной расстановке
            if (isPlacingEntranceExit)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.DeepSkyBlue));
                foreach (var pt in availableEntranceExitCells)
                    g.FillRectangle(brush, pt.X * w, pt.Y * h, w, h);
            }

            // Подсветка выбранного входа
            if (isPlacingEntranceExit)
            {
                if (manualEntrance.HasValue)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.LimeGreen)), manualEntrance.Value.X * w, manualEntrance.Value.Y * h, w, h);
                if (manualExit.HasValue)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), manualExit.Value.X * w, manualExit.Value.Y * h, w, h);
            }
        }

        // Клик для ручной расстановки
        private void PnlMazeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isPlacingEntranceExit || currentMaze == null) return;
            int cellW = pnlMazeView.Width / currentMaze.Width;
            int cellH = pnlMazeView.Height / currentMaze.Height;
            int x = e.X / cellW;
            int y = e.Y / cellH;
            var pt = new Point(x, y);

            if (!availableEntranceExitCells.Contains(pt)) return;

            if (placementStep == 0)
            {
                manualEntrance = pt;
                placementStep = 1;
                pnlMazeView.Invalidate();
            }
            else if (placementStep == 1)
            {
                manualExit = pt;
                try
                {
                    mazeService.PlaceEntranceExit(currentMaze, false, manualEntrance, manualExit);
                    isPlacingEntranceExit = false;
                    placementStep = 0;
                    pnlMazeView.Invalidate();
                    Controls.Find("btnGenerateMaze", true)[0].Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    placementStep = 0;
                    manualEntrance = null;
                    manualExit = null;
                    pnlMazeView.Invalidate();
                }
            }
        }

        private void GenerateMaze()
        {
            if (currentMaze == null)
            {
                MessageBox.Show("Сначала создайте шаблон лабиринта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (currentMaze.Entrance == Point.Empty || currentMaze.Exit == Point.Empty)
            {
                MessageBox.Show("Сначала расставьте вход и выход.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rbPrim = Controls.Find("rbPrim", true)[0] as RadioButton;
            string algorithm = rbPrim.Checked ? "Прима" : "Эллера";

            try
            {
                mazeService.GenerateMaze(currentMaze, algorithm);
                pnlMazeView.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка генерации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMaze()
        {
            var form = new SaveMazeForm(db, currentMaze);
            form.ShowDialog(this);
        }

        private void ShowAbout()
        {
            var form = new AboutForm();
            form.ShowDialog(this);
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
        }
    }
}