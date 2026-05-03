using System;
using System.Windows.Forms;

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
            this.Text = "Справочная информация";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Справка",
                Font = new System.Drawing.Font("Segoe UI", 16),
                Location = new System.Drawing.Point(240, 10),
                Size = new System.Drawing.Size(80, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Обучение
            var lblUniver = new Label
            {
                Text = "Самарский университет\nИнститут информатики и кибернетики",
                Font = new System.Drawing.Font("Segoe UI", 12),
                Location = new System.Drawing.Point(150, 50),
                Size = new System.Drawing.Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblUniver);

            // Название
            var lblName = new Label
            {
                Text = "Курсовой проект по дисциплине “Программная инженерия”\nпо теме “Автоматизированная система генерации лабиринта и \nнахождения выхода из него”",
                Font = new System.Drawing.Font("Segoe UI", 12),
                Location = new System.Drawing.Point(60, 120),
                Size = new System.Drawing.Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblName);

            // Разработчики
            var lblAuthor = new Label
            {
                Text = "Разработчики (обучающиеся группы 6302-020302D):\nКудряшов К. В.\nМрясов С. В.\n\n2026 г.",
                Font = new System.Drawing.Font("Segoe UI", 12),
                Location = new System.Drawing.Point(100, 210),
                Size = new System.Drawing.Size(150, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblAuthor);

            // Кнопка О системе
            var btnInfo = new Button
            {
                Text = "О системе",
                Location = new System.Drawing.Point(200, 330),
                Size = new System.Drawing.Size(200, 40),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnInfo.Click += (s, e) => BtnInfo_Click();
            this.Controls.Add(btnInfo);
        }

        private void BtnInfo_Click()
        {
            // Переход на HTML-страницу с информацией о системе
        }
    }
}