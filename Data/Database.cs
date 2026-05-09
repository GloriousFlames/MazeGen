using Microsoft.Data.Sqlite;
using MazeGen.Models;

namespace MazeGen.Data
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string dbPath = "mazegen.db")
        {
            _connectionString = $"Data Source={dbPath}";
            EnsureTables();
        }

        private void EnsureTables()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login VARCHAR(30) NOT NULL UNIQUE,
                    PasswordHash VARCHAR(20) NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Mazes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name VARCHAR(30) NOT NULL,
                    Width INTEGER NOT NULL,
                    Height INTEGER NOT NULL,
                    EntranceX INTEGER NOT NULL,
                    EntranceY INTEGER NOT NULL,
                    ExitX INTEGER NOT NULL,
                    ExitY INTEGER NOT NULL,
                    Grid BLOB NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();
        }

        // Пользователь
        public void AddUser(User user)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Login, PasswordHash) VALUES (@login, @hash)";
            cmd.Parameters.AddWithValue("@login", user.Login);
            cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }

        public User? GetUser(string login)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Login, PasswordHash FROM Users WHERE Login = @login";
            cmd.Parameters.AddWithValue("@login", login);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Login = reader.GetString(1),
                    PasswordHash = reader.GetString(2)
                };
            }
            return null;
        }

        // Лабиринт
        public void SaveMaze(Maze maze)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Mazes (Name, Width, Height, EntranceX, EntranceY, ExitX, ExitY, Grid)
                VALUES (@name, @w, @h, @ex, @ey, @xx, @xy, @grid)";
            cmd.Parameters.AddWithValue("@name", maze.Name ?? "");
            cmd.Parameters.AddWithValue("@w", maze.Width);
            cmd.Parameters.AddWithValue("@h", maze.Height);
            cmd.Parameters.AddWithValue("@ex", maze.Entrance.X);
            cmd.Parameters.AddWithValue("@ey", maze.Entrance.Y);
            cmd.Parameters.AddWithValue("@xx", maze.Exit.X);
            cmd.Parameters.AddWithValue("@xy", maze.Exit.Y);
            cmd.Parameters.AddWithValue("@grid", GridToBytes(maze.Grid));
            cmd.ExecuteNonQuery();
        }

        public List<Maze> GetAllMazes()
        {
            var result = new List<Maze>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Width, Height, EntranceX, EntranceY, ExitX, ExitY, Grid FROM Mazes";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int w = reader.GetInt32(2), h = reader.GetInt32(3);
                result.Add(new Maze
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Width = w,
                    Height = h,
                    Entrance = new Point(reader.GetInt32(4), reader.GetInt32(5)),
                    Exit = new Point(reader.GetInt32(6), reader.GetInt32(7)),
                    Grid = BytesToGrid((byte[])reader["Grid"], w, h)
                });
            }
            return result;
        }

        // Сохранение сетки лабиринта
        private static byte[] GridToBytes(int[,] grid)
        {
            int w = grid.GetLength(0), h = grid.GetLength(1);
            var bytes = new byte[w * h * sizeof(int)];
            Buffer.BlockCopy(grid, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        // Загрузка сетки лабиринта
        private static int[,] BytesToGrid(byte[] bytes, int w, int h)
        {
            var grid = new int[w, h];
            Buffer.BlockCopy(bytes, 0, grid, 0, bytes.Length);
            return grid;
        }
    }
}