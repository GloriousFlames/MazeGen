using MazeGen.Data;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class SaveMazeForm : Form
    {
        private MazeService mazeService;
        private Maze mazeToSave;
        private Database db;

        public SaveMazeForm(Database db, Maze mazeToSave)
        {
            this.db = db;
            this.mazeToSave = mazeToSave;
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.Text = "Сохранение лабиринта";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Сохранение лабиринта",
                Font = new System.Drawing.Font("Segoe UI", 16),
                Location = new System.Drawing.Point(100, 10),
                Size = new System.Drawing.Size(80, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var lblMazes = new Label
            {
                Text = "Введите название:",
                Location = new System.Drawing.Point(20, 60),
                Font = new System.Drawing.Font("Segoe UI", 12),
                Size = new System.Drawing.Size(150, 20),
                AutoSize = true
            };
            this.Controls.Add(lblMazes);

            var txtName = new TextBox
            {
                Name = "txtName",
                Location = new System.Drawing.Point(180, 60),
                Size = new System.Drawing.Size(180, 20)
            };
            this.Controls.Add(txtName);

            var btnSave = new Button
            {
                Text = "Сохранить",
                Location = new System.Drawing.Point(150, 100),
                Size = new System.Drawing.Size(100, 40),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            btnSave.Click += (s, e) => Save();
            this.Controls.Add(btnSave);
        }

        private void Save()
        {
            var txtName = Controls["txtName"] as TextBox;
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название лабиринта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string mazeName = txtName.Text.Trim();

            if (db.GetAllMazes().Any(m => string.Equals(m.Name, mazeName)))
            {
                MessageBox.Show("Лабиринт с таким именем уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            mazeToSave.Name = mazeName;
            db.SaveMaze(mazeToSave);
            MessageBox.Show($"Лабиринт '{mazeName}' сохранен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}