namespace MazeGen
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Справочная информация";
            Size = new Size(600, 450);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Справка",
                Font = new Font("Segoe UI", 16),
                Location = new Point(240, 10),
                Size = new Size(80, 30),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            // Обучение
            var lblUniver = new Label
            {
                Text = "Самарский университет\nИнститут информатики и кибернетики",
                Font = new Font("Segoe UI", 12),
                Location = new Point(150, 50),
                Size = new Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblUniver);

            // Название
            var lblName = new Label
            {
                Text = "Курсовой проект по дисциплине “Программная инженерия”\nпо теме “Автоматизированная система генерации лабиринта и \nнахождения выхода из него”",
                Font = new Font("Segoe UI", 12),
                Location = new Point(60, 120),
                Size = new Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblName);

            // Разработчики
            var lblAuthor = new Label
            {
                Text = "Разработчики (обучающиеся группы 6302-020302D):\nКудряшов К. В.\nМрясов С. В.\n\n2026 г.",
                Font = new Font("Segoe UI", 12),
                Location = new Point(100, 210),
                Size = new Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblAuthor);
        }
    }
}