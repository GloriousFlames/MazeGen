using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class AdminMainForm : Form
    {
        private User currentUser;
        private MazeService mazeService;
        private Theme currentTheme = Theme.Forest;

        public AdminMainForm(User user)
        {
            currentUser = user;
            mazeService = new MazeService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"MazeGen - Администратор ({currentUser.Login})";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Меню
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("Файл");
            var saveMazeItem = new ToolStripMenuItem("Сохранить лабиринт");
            saveMazeItem.Click += (s, e) => SaveMaze();
            fileMenu.DropDownItems.Add(saveMazeItem);
            var exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => this.Close();
            fileMenu.DropDownItems.Add(exitItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            var aboutItem = new ToolStripMenuItem("О программе");
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

            // Блок параметров лабиринта
            var paramsGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 125),
                Size = new System.Drawing.Size(320, 100)
            };

            var lblParams = new Label
            {
                Text = "Параметры лабиринта",
                Location = new System.Drawing.Point(10, 110),
                Size = new System.Drawing.Size(180, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
            };
            pnlLeft.Controls.Add(lblParams);

            var lblWidth = new Label 
            { 
                Text = "Ширина", 
                Location = new System.Drawing.Point(120, 20), 
                Size = new System.Drawing.Size(80, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            paramsGroup.Controls.Add(lblWidth);

            var numWidth = new NumericUpDown 
            { 
                Name = "numWidth", 
                Value = 15, 
                Minimum = 7, 
                Maximum = 25, 
                Location = new System.Drawing.Point(200, 20), 
                Size = new System.Drawing.Size(100, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            paramsGroup.Controls.Add(numWidth);
            
            var lblHeight = new Label 
            { 
                Text = "Длина", 
                Location = new System.Drawing.Point(120, 60), 
                Size = new System.Drawing.Size(80, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
            };
            paramsGroup.Controls.Add(lblHeight);
            
            var numHeight = new NumericUpDown 
            { 
                Name = "numHeight", 
                Value = 15, 
                Minimum = 7, 
                Maximum = 21, 
                Location = new System.Drawing.Point(200, 60), 
                Size = new System.Drawing.Size(100, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
            };
            paramsGroup.Controls.Add(numHeight);
            
            pnlLeft.Controls.Add(paramsGroup);

            var btnCreateTemplate = new Button
            {
                Text = "Создать шаблон",
                Location = new System.Drawing.Point(70, 235),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnCreateTemplate.Click += (s, e) => CreateTemplate();
            pnlLeft.Controls.Add(btnCreateTemplate);

            // Блок расстановки входа/выхода
            var lblPlacement = new Label
            {
                Text = "Расстановка входа/выхода:",
                Location = new System.Drawing.Point(10, 280),
                Size = new System.Drawing.Size(250, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblPlacement);

            var placementGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 300),
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

            var btnApplyPlacement = new Button
            {
                Text = "Применить",
                Location = new System.Drawing.Point(100, 360),
                Size = new System.Drawing.Size(150, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnApplyPlacement.Click += (s, e) => ApplyPlacement();
            pnlLeft.Controls.Add(btnApplyPlacement);

            // Блок выбора алгоритма
            var lblAlgorithm = new Label
            {
                Text = "Алгоритм генерации",
                Location = new System.Drawing.Point(10, 410),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            pnlLeft.Controls.Add(lblAlgorithm);

            var algorithmGroup = new GroupBox
            {
                Text = "",
                Location = new System.Drawing.Point(10, 420),
                Size = new System.Drawing.Size(320, 50)
            };

            var rbPrim = new RadioButton
            {
                Text = "Прима",
                Location = new System.Drawing.Point(20, 15),
                Checked = true,
                AutoSize = true,
                Name = "rbPrim",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };

            var rbEller = new RadioButton
            {
                Text = "Эллера",
                Location = new System.Drawing.Point(200, 15),
                AutoSize = true,
                Name = "rbEller",
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            algorithmGroup.Controls.Add(rbPrim);
            algorithmGroup.Controls.Add(rbEller);
            pnlLeft.Controls.Add(algorithmGroup);

            var btnGenerateMaze = new Button
            {
                Text = "Создать лабиринт",
                Location = new System.Drawing.Point(70, 480),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnGenerateMaze.Click += (s, e) => GenerateMaze();
            pnlLeft.Controls.Add(btnGenerateMaze);

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

        private void CreateTemplate()
        {
            MessageBox.Show("Шаблон лабиринта создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyPlacement()
        {
            MessageBox.Show("Параметры расстановки применены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateMaze()
        {
            MessageBox.Show("Лабиринт сгенерирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveMaze()
        {
            var form = new SaveMazeForm();
            form.ShowDialog(this);
        }

        private void ShowAbout()
        {
            var form = new AboutForm();
            form.ShowDialog(this);
        }
    }
}