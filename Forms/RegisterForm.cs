using MazeGen.Data;
using MazeGen.Services;

namespace MazeGen
{
    public partial class RegisterForm : Form
    {
        private AuthenticationService authService;
        private Database db;

        public RegisterForm(Database db)
        {
            InitializeComponent();
            authService = new AuthenticationService(db);
            this.db = db;
        }

        private void InitializeComponent()
        {
            Text = "MazeGen - Регистрация";
            Size = new Size(450, 320);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Регистрация",
                Font = new Font("Segoe UI", 16),
                Location = new Point(150, 10),
                Size = new Size(150, 30),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            // Логин
            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new Point(123, 70),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 12)
            };
            Controls.Add(lblLogin);

            var txtLogin = new TextBox
            {
                Name = "txtLogin",
                Location = new Point(200, 70),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12)
            };
            Controls.Add(txtLogin);

            // Пароль
            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(115, 110),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 12)
            };
            Controls.Add(lblPassword);

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(200, 110),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            Controls.Add(txtPassword);

            // Подтверждение пароля
            var lblConfirm = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(20, 150),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 12)
            };
            Controls.Add(lblConfirm);

            var txtConfirm = new TextBox
            {
                Name = "txtConfirm",
                Location = new Point(200, 150),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            Controls.Add(txtConfirm);

            // Статус сообщение
            var lblStatus = new Label
            {
                Name = "lblStatus",
                Location = new Point(30, 190),
                Size = new Size(340, 30),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblStatus);

            // Кнопка Зарегистрироваться
            var btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(100, 220),
                Size = new Size(200, 40),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnRegister.Click += (s, e) => BtnRegister_Click();
            Controls.Add(btnRegister);
        }

        private void BtnRegister_Click()
        {
            var txtLogin = Controls["txtLogin"] as TextBox;
            var txtPassword = Controls["txtPassword"] as TextBox;
            var txtConfirm = Controls["txtConfirm"] as TextBox;
            var lblStatus = Controls["lblStatus"] as Label;

            string login = txtLogin.Text;
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (login.Length < 4 || login.Length > 16)
            {
                lblStatus.Text = "Логин должен быть от 4 до 16 символов.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            if (password.Length < 4 || password.Length > 16)
            {
                lblStatus.Text = "Пароль должен быть от 4 до 16 символов.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            if (password != confirm)
            {
                lblStatus.Text = "Пароли не совпадают!";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            bool success = authService.Register(login, password);
            if (success)
            {
                lblStatus.Text = "Регистрация успешна!";
                lblStatus.ForeColor = Color.Green;
                Thread.Sleep(1000);
                Close();
            }
            else
            {
                lblStatus.Text = "Ошибка регистрации! Проверьте данные.";
                lblStatus.ForeColor = Color.Red;
            }
        }
    }
}