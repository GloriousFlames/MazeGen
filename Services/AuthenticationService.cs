using MazeGen.Models;

namespace MazeGen.Services
{
    public class AuthenticationService
    {
        private List<User> users = new List<User>();
        private int nextUserId = 1;

        public AuthenticationService()
        {
            // Тестовый администратор
            users.Add(new User
            {
                Id = nextUserId++,
                Login = "admin",
                PasswordHash = HashPassword("admin"),
                Role = UserRole.Admin
            });
        }

        public User Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return null;

            foreach (var user in users)
            {
                if (user.Login == login && VerifyPassword(password, user.PasswordHash))
                    return user;
            }
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

            foreach (var user in users)
            {
                if (user.Login == login)
                    return false;
            }

            users.Add(new User
            {
                Id = nextUserId++,
                Login = login,
                PasswordHash = HashPassword(password),
                Role = UserRole.Player
            });
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