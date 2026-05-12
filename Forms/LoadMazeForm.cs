using MazeGen.Data;
using MazeGen.Services;

namespace MazeGen
{
    public partial class LoadMazeForm : Form
    {
        private Database db;
        private PlayerMainForm playerForm;

        public LoadMazeForm(Database db, PlayerMainForm playerForm)
        {
            this.db = db;
            this.playerForm = playerForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Загрузить лабиринт";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Загрузка лабиринта",
                Font = new Font("Segoe UI", 16),
                Location = new Point(100, 10),
                Size = new Size(80, 30),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            var lblMazes = new Label
            {
                Text = "Выберите лабиринт:",
                Location = new Point(20, 60),
                Font = new Font("Segoe UI", 12),
                Size = new Size(150, 20),
                AutoSize = true
            };
            Controls.Add(lblMazes);

            var cmbMazes = new ComboBox
            {
                Name = "cmbMazes",
                Location = new Point(200, 60),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            var mazes = db.GetAllMazes();
            cmbMazes.Items.Clear();
            foreach (var maze in mazes)
            {
                cmbMazes.Items.Add(maze.Name);
            }
            Controls.Add(cmbMazes);

            var btnLoad = new Button
            {
                Text = "Загрузить",
                Location = new Point(150, 100),
                Size = new Size(100, 40),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12)
            };
            btnLoad.Click += (s, e) => Load();
            Controls.Add(btnLoad);
        }

        private void Load()
        {
            var cmb = Controls["cmbMazes"] as ComboBox;
            if (cmb.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите лабиринт!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedMaze = db.GetAllMazes().FirstOrDefault(m => m.Name == cmb.SelectedItem.ToString());
            if (selectedMaze != null)
            {
                playerForm.LoadMazeFromForm(selectedMaze);
                MessageBox.Show($"Лабиринт '{cmb.SelectedItem}' загружен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Лабиринт не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}