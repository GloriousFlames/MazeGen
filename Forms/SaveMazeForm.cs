using MazeGen.Data;
using MazeGen.Models;
using MazeGen.Services;

namespace MazeGen
{
    public partial class SaveMazeForm : Form
    {
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
            Text = "Сохранение лабиринта";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Сохранение лабиринта",
                Font = new Font("Segoe UI", 16),
                Location = new Point(80, 10),
                Size = new Size(80, 30),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            var lblMazes = new Label
            {
                Text = "Введите название:",
                Location = new Point(20, 60),
                Font = new Font("Segoe UI", 12),
                Size = new Size(150, 20),
                AutoSize = true
            };
            Controls.Add(lblMazes);

            var txtName = new TextBox
            {
                Name = "txtName",
                Location = new Point(180, 60),
                Size = new Size(180, 20)
            };
            Controls.Add(txtName);

            var btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(150, 100),
                Size = new Size(100, 40),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12)
            };
            btnSave.Click += (s, e) => Save();
            Controls.Add(btnSave);
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