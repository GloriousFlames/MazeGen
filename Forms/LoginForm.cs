using MazeGen.Services;
using MazeGen.Models;
using MazeGen.Data;

namespace MazeGen
{
    public partial class LoginForm : Form
    {
        private AuthenticationService authService;
        private MazeService mazeService;
        private Database db;

        public LoginForm(Database db, MazeService ms)
        {
            InitializeComponent();
            authService = new AuthenticationService(db);
            this.db = db;
            mazeService = ms;
        }

        private void InitializeComponent()
        {
            this.Text = "MazeGen - Вход";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Авторизация",
                Font = new Font("Segoe UI", 16),
                Location = new Point(130, 10),
                Size = new Size(100, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Логин
            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new Point(30, 70),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 12)
            };
            this.Controls.Add(lblLogin);

            var txtLogin = new TextBox
            {
                Name = "txtLogin",
                Location = new Point(120, 70),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12)
            };
            this.Controls.Add(txtLogin);

            // Пароль
            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(30, 110),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 12)
            };
            this.Controls.Add(lblPassword);

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(120, 110),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            this.Controls.Add(txtPassword);

            // Кнопка Войти
            var btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(130, 220),
                Size = new Size(120, 40),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnLogin.Click += (s, e) => BtnLogin_Click();
            Controls.Add(btnLogin);

            // Кнопка Регистрация
            var btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(30, 150),
                Size = new Size(120, 40),
                BackColor = Color.Ivory,
                Font = new Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnRegister.Click += (s, e) => BtnRegister_Click();
            Controls.Add(btnRegister);

            // Статус сообщение
            var lblStatus = new Label
            {
                Name = "lblStatus",
                Location = new Point(30, 190),
                Size = new Size(340, 40),
                AutoSize = false,
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblStatus);
        }

        private void BtnLogin_Click()
        {
            var txtLogin = Controls["txtLogin"] as TextBox;
            var txtPassword = Controls["txtPassword"] as TextBox;
            var lblStatus = Controls["lblStatus"] as Label;

            string login = txtLogin.Text;
            string password = txtPassword.Text;

            var user = authService.Login(login, password);
            if (user != null)
            {
                Hide();
                if (user.Login.Equals("admin"))
                {
                    LoginAsAdmin(user);
                }
                else
                {
                    LoginAsPlayer(user);
                }
            }
            else
            {
                lblStatus.Text = "Неверные логин или пароль!";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnRegister_Click()
        {
            var registerForm = new RegisterForm(db);
            registerForm.ShowDialog(this);
        }

        private void LoginAsAdmin(User user)
        {
            var adminForm = new AdminMainForm(user, db, mazeService);
            adminForm.Show();
            Hide();
            adminForm.FormClosed += (s, e) => Show();
        }

        private void LoginAsPlayer(User user)
        {
            var playerForm = new PlayerMainForm(user, db, mazeService);
            playerForm.Show();
            Hide();
            playerForm.FormClosed += (s, e) => Show();
        }
    }
}