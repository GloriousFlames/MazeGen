using System.Collections.Generic;
using MazeGen.Models;

namespace MazeGen.Services
{
    public class MazeService
    {
        private List<Maze> mazes = new List<Maze>();

        public Maze GenerateMaze(int width, int height, Theme theme, string algorithm)
        {
            // Заглушка для генерации лабиринта
            var maze = new Maze
            {
                Id = mazes.Count + 1,
                Width = width,
                Height = height,
                Theme = theme,
                Grid = new int[width, height]
            };
            mazes.Add(maze);
            return maze;
        }

        public List<Maze> GetAllMazes()
        {
            return mazes;
        }

        public void SaveMaze(Maze maze)
        {
            // Заглушка для сохранения лабиринта
            mazes.Add(maze);
        }
    }
}