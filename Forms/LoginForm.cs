using System;
using System.Windows.Forms;
using MazeGen.Services;
using MazeGen.Models;

namespace MazeGen
{
    public partial class LoginForm : Form
    {
        private AuthenticationService authService;
        private MazeService mazeService;

        public LoginForm(MazeService mazeService)
        {
            this.mazeService = mazeService;
            InitializeComponent();
            authService = new AuthenticationService();
        }

        private void InitializeComponent()
        {
            this.Text = "MazeGen - Вход";
            this.Size = new System.Drawing.Size(400, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Авторизация",
                Font = new System.Drawing.Font("Segoe UI", 16),
                Location = new System.Drawing.Point(130, 10),
                Size = new System.Drawing.Size(100, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Логин
            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new System.Drawing.Point(30, 70),
                Size = new System.Drawing.Size(80, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(lblLogin);

            var txtLogin = new TextBox
            {
                Name = "txtLogin",
                Location = new System.Drawing.Point(120, 70),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(txtLogin);

            // Пароль
            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new System.Drawing.Point(30, 110),
                Size = new System.Drawing.Size(80, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(lblPassword);

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new System.Drawing.Point(120, 110),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            this.Controls.Add(txtPassword);

            // Кнопка Войти
            var btnLogin = new Button
            {
                Text = "Войти",
                Location = new System.Drawing.Point(130, 220),
                Size = new System.Drawing.Size(120, 40),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnLogin.Click += (s, e) => BtnLogin_Click();
            this.Controls.Add(btnLogin);

            // Кнопка Регистрация
            var btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new System.Drawing.Point(30, 150),
                Size = new System.Drawing.Size(120, 40),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnRegister.Click += (s, e) => BtnRegister_Click();
            this.Controls.Add(btnRegister);

            // Статус сообщение
            var lblStatus = new Label
            {
                Name = "lblStatus",
                Location = new System.Drawing.Point(30, 220),
                Size = new System.Drawing.Size(340, 40),
                AutoSize = false,
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblStatus);
        }

        private void BtnLogin_Click()
        {
            var txtLogin = this.Controls["txtLogin"] as TextBox;
            var txtPassword = this.Controls["txtPassword"] as TextBox;
            var lblStatus = this.Controls["lblStatus"] as Label;

            string login = txtLogin.Text;
            string password = txtPassword.Text;

            var user = authService.Login(login, password);
            if (user != null)
            {
                this.Hide();
                if (user.Id == 1)
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
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnRegister_Click()
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog(this);
        }

        private void LoginAsAdmin(User user)
        {
            var adminForm = new AdminMainForm(user, mazeService);
            adminForm.Show();
            this.Hide();
            //adminForm.FormClosed += (s, e) => this.Show();
        }

        private void LoginAsPlayer(User user)
        {
            var playerForm = new PlayerMainForm(user, mazeService);
            playerForm.Show();
            this.Hide();
            playerForm.FormClosed += (s, e) => this.Show();
        }
    }
}