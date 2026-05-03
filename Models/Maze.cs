namespace MazeGen.Models
{
    public class Maze
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[,] Grid { get; set; }
        public System.Drawing.Point Entrance { get; set; }
        public System.Drawing.Point Exit { get; set; }
        public Theme Theme { get; set; }
        public string CreatedDate { get; set; }
    }

    public enum Theme
    {
        Forest,
        Plain,
        Mountains,
        Desert
    }
}