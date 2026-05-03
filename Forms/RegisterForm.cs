using System;
using System.Windows.Forms;
using MazeGen.Services;

namespace MazeGen
{
    public partial class RegisterForm : Form
    {
        private AuthenticationService authService;

        public RegisterForm()
        {
            InitializeComponent();
            authService = new AuthenticationService();
        }

        private void InitializeComponent()
        {
            this.Text = "MazeGen - Регистрация";
            this.Size = new System.Drawing.Size(450, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Регистрация",
                Font = new System.Drawing.Font("Segoe UI", 16),
                Location = new System.Drawing.Point(150, 10),
                Size = new System.Drawing.Size(150, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Логин
            var lblLogin = new Label
            {
                Text = "Логин:",
                Location = new System.Drawing.Point(123, 70),
                Size = new System.Drawing.Size(60, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(lblLogin);

            var txtLogin = new TextBox
            {
                Name = "txtLogin",
                Location = new System.Drawing.Point(200, 70),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(txtLogin);

            // Пароль
            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new System.Drawing.Point(115, 110),
                Size = new System.Drawing.Size(80, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(lblPassword);

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new System.Drawing.Point(200, 110),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            this.Controls.Add(txtPassword);

            // Подтверждение пароля
            var lblConfirm = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new System.Drawing.Point(20, 150),
                Size = new System.Drawing.Size(180, 20),
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            this.Controls.Add(lblConfirm);

            var txtConfirm = new TextBox
            {
                Name = "txtConfirm",
                Location = new System.Drawing.Point(200, 150),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Segoe UI", 12),
                PasswordChar = '*'
            };
            this.Controls.Add(txtConfirm);

            // Статус сообщение
            var lblStatus = new Label
            {
                Name = "lblStatus",
                Location = new System.Drawing.Point(30, 220),
                Size = new System.Drawing.Size(340, 30),
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            //this.Controls.Add(lblStatus);

            // Кнопка Зарегистрироваться
            var btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new System.Drawing.Point(100, 220),
                Size = new System.Drawing.Size(200, 40),
                BackColor = System.Drawing.Color.Ivory,
                Font = new System.Drawing.Font("Segoe UI", 12),
                Cursor = Cursors.Hand
            };
            btnRegister.Click += (s, e) => BtnRegister_Click();
            this.Controls.Add(btnRegister);
        }

        private void BtnRegister_Click()
        {
            var txtLogin = this.Controls["txtLogin"] as TextBox;
            var txtPassword = this.Controls["txtPassword"] as TextBox;
            var txtConfirm = this.Controls["txtConfirm"] as TextBox;
            var lblStatus = this.Controls["lblStatus"] as Label;

            string login = txtLogin.Text;
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (password != confirm)
            {
                lblStatus.Text = "Пароли не совпадают!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            bool success = authService.Register(login, password);
            if (success)
            {
                lblStatus.Text = "Регистрация успешна!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                System.Threading.Thread.Sleep(1000);
                this.Close();
            }
            else
            {
                lblStatus.Text = "Ошибка регистрации! Проверьте данные.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}