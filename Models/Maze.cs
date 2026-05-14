namespace MazeGen.Models
{
    public class Maze
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[,] Grid { get; set; }
        public Point Entrance { get; set; }
        public Point Exit { get; set; }
    }
}