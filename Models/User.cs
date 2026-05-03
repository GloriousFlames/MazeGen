namespace MazeGen.Models
{
    public enum UserRole
    {
        Admin,
        Player
    }

    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }
}