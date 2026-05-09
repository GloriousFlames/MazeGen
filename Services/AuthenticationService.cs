using MazeGen.Models;
using MazeGen.Data;

namespace MazeGen.Services
{
    public class AuthenticationService
    {
        private List<User> users = new List<User>();
        private int nextUserId = 1;
        private Database db;

        public AuthenticationService(Database db)
        {
            this.db = db;
            // Администратор
            users.Add(new User
            {
                Id = nextUserId++,
                Login = "admin",
                PasswordHash = HashPassword("admin"),
            });
        }

        public User Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return null;

            if (login.Equals("admin") && VerifyPassword(password, users[0].PasswordHash))
                return users[0];

            var user = db.GetUser(login);
            if (user != null && VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }

        public bool Register(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            if (login.Length < 4 || login.Length > 16)
                return false;

            if (password.Length < 4 || password.Length > 16)
                return false;

            if (db.GetUser(login) != null)
                return false;

            db.AddUser(new User { Login = login, PasswordHash = HashPassword(password) });
            return true;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password)
            );
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}