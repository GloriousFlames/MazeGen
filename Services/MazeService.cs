using MazeGen.Models;

namespace MazeGen.Services
{
    public class MazeService
    {
        private List<Maze> mazes = new List<Maze>();
        private Random rand = new Random();
        public Maze CreateTemplate(int width, int height)
        {
            if (width % 2 == 0) width++;
            if (height % 2 == 0) height++;
            var grid = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    grid[x, y] = 0;

            return new Maze
            {
                Id = mazes.Count + 1,
                Width = width,
                Height = height,
                Grid = grid,
                //CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public void PlaceEntranceExit(Maze maze, bool auto, Point? manualEntrance = null, Point? manualExit = null)
        {
            // Очистить предыдущие вход и выход
            if (maze.Entrance != Point.Empty)
                maze.Grid[maze.Entrance.X, maze.Entrance.Y] = 0;
            if (maze.Exit != Point.Empty)
                maze.Grid[maze.Exit.X, maze.Exit.Y] = 0;

            if (auto)
            {
                var perim = GetPerimeterCells(maze.Width, maze.Height);
                Point entrance, exit;
                do
                {
                    entrance = perim[rand.Next(perim.Count)];
                    exit = perim[rand.Next(perim.Count)];
                }
                while (!IsValidEntranceExit(entrance, exit, maze.Width, maze.Height));
                maze.Entrance = entrance;
                maze.Exit = exit;
            }
            else
            {
                if (manualEntrance == null || manualExit == null)
                    throw new ArgumentException("Неверные позиции входа или выхода!");
                if (!IsValidEntranceExit(manualEntrance.Value, manualExit.Value, maze.Width, maze.Height))
                    throw new ArgumentException("Неверные позиции входа или выхода!");
                maze.Entrance = manualEntrance.Value;
                maze.Exit = manualExit.Value;
            }
            maze.Grid[maze.Entrance.X, maze.Entrance.Y] = 1;
            maze.Grid[maze.Exit.X, maze.Exit.Y] = 1;
        }

        private List<Point> GetPerimeterCells(int width, int height)
        {
            var list = new List<Point>();
            for (int x = 0; x < width; x++)
            {
                if (x != 0 && x != width - 1)
                {
                    list.Add(new Point(x, 0));
                    list.Add(new Point(x, height - 1));
                }
            }
            for (int y = 0; y < height; y++)
            {
                if (y != 0 && y != height - 1)
                {
                    list.Add(new Point(0, y));
                    list.Add(new Point(width - 1, y));
                }
            }
            return list;
        }

        private bool IsValidEntranceExit(Point a, Point b, int width, int height)
        {
            if (!IsValidPerimeterPoint(a, width, height)) return false;
            if (!IsValidPerimeterPoint(b, width, height)) return false;

            // Если не соседи
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) >= 2;
        }

        private bool IsValidPerimeterPoint(Point p, int width, int height)
        {
            // Левая/правая стена: X фиксирован, Y нечётный и не угловой
            bool onLeft = p.X == 0 && p.Y % 2 == 1 && p.Y < height - 1;
            bool onRight = p.X == width - 1 && p.Y % 2 == 1 && p.Y < height - 1;
            // Верхняя/нижняя стена: Y фиксирован, X нечётный и не угловой
            bool onTop = p.Y == 0 && p.X % 2 == 1 && p.X < width - 1;
            bool onBottom = p.Y == height - 1 && p.X % 2 == 1 && p.X < width - 1;

            return onLeft || onRight || onTop || onBottom;
        }

        public void GenerateMaze(Maze maze, string algorithm)
        {
            if (algorithm == "Прима")
                GeneratePrim(maze);
            else if (algorithm == "Эллера")
                GenerateEller(maze);
        }

        // Алгоритм Прима
        private void GeneratePrim(Maze maze)
        {
            int w = maze.Width, h = maze.Height;
            var grid = maze.Grid;

            // Все клетки - стены
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    grid[i, j] = 0;
                }
            }

            // Выбираем случайную клетку с нечетными координатами и очищаем её
            int x = rand.Next(w / 2) * 2 + 1;
            int y = rand.Next(h / 2) * 2 + 1;
            grid[x, y] = 1;

            // Создаем список клеток для проверки - клетки на расстоянии 2
            var toCheck = new List<Point>();
            AddNeighborsToCheck(toCheck, x, y, w, h, grid);

            while (toCheck.Count > 0)
            {
                // Выбираем случайную клетку
                int index = rand.Next(toCheck.Count);
                Point cell = toCheck[index];
                x = cell.X;
                y = cell.Y;
                grid[x, y] = 1;
                toCheck.RemoveAt(index);

                // Ищем соседнюю уже очищенную клетку и соединяем их
                var directions = new List<(int dx, int dy)> { (0, -2), (0, 2), (-2, 0), (2, 0) };
                
                bool connected = false;
                while (directions.Count > 0 && !connected)
                {
                    int dirIndex = rand.Next(directions.Count);
                    var (dx, dy) = directions[dirIndex];
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < w && ny >= 0 && ny < h && grid[nx, ny] == 1)
                    {
                        // Очищаем клетку между ними
                        grid[x + dx / 2, y + dy / 2] = 1;
                        connected = true;
                    }

                    directions.RemoveAt(dirIndex);
                }

                // Добавляем соседние клетки на расстоянии 2, которые еще стены
                AddNeighborsToCheck(toCheck, x, y, w, h, grid);
            }

            grid[maze.Entrance.X, maze.Entrance.Y] = 1;
            grid[maze.Exit.X, maze.Exit.Y] = 1;
        }

        private void AddNeighborsToCheck(List<Point> toCheck, int x, int y, int w, int h, int[,] grid)
        {
            if (y - 2 >= 0 && grid[x, y - 2] == 0 && !toCheck.Contains(new Point(x, y - 2)))
                toCheck.Add(new Point(x, y - 2));
            if (y + 2 < h && grid[x, y + 2] == 0 && !toCheck.Contains(new Point(x, y + 2)))
                toCheck.Add(new Point(x, y + 2));
            if (x - 2 >= 0 && grid[x - 2, y] == 0 && !toCheck.Contains(new Point(x - 2, y)))
                toCheck.Add(new Point(x - 2, y));
            if (x + 2 < w && grid[x + 2, y] == 0 && !toCheck.Contains(new Point(x + 2, y)))
                toCheck.Add(new Point(x + 2, y));
        }

        // Алгоритм Эллера
        private void GenerateEller(Maze maze)
        {
            int w = maze.Width, h = maze.Height;
            var grid = maze.Grid;

            // Все клетки — стены
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    grid[x, y] = 0;

            var cellToSet = new Dictionary<int, int>();
            int nextSetId = 1;

            for (int row = 1; row < h - 1; row += 2)
            {
                // Открыть все ячейки строки
                for (int col = 1; col < w - 1; col += 2)
                    grid[col, row] = 1;

                // Присвоить множество ячейкам, у которых его ещё нет
                for (int col = 1; col < w - 1; col += 2)
                    if (!cellToSet.ContainsKey(col))
                        cellToSet[col] = nextSetId++;

                // Случайно создавать/удалять правые стены
                for (int col = 1; col < w - 3; col += 2)
                {
                    int set1 = cellToSet[col];
                    int set2 = cellToSet[col + 2];

                    if (set1 == set2)
                    {
                        grid[col + 1, row] = 0;
                    }
                    else if (rand.Next(2) == 0)
                    {
                        grid[col + 1, row] = 1;
                        MergeSets(cellToSet, set1, set2);
                    }
                    else
                    {
                        grid[col + 1, row] = 0;
                    }
                }

                // Если не последняя строка — создать нижние стены
                if (row < h - 2)
                {
                    // Счётчик открытых проходов вниз для каждого множества
                    var setDownCount = new Dictionary<int, int>();
                    for (int col = 1; col < w - 1; col += 2)
                    {
                        int set = cellToSet[col];
                        if (!setDownCount.ContainsKey(set))
                            setDownCount[set] = 0;
                    }

                    // Случайно расставить нижние стены
                    for (int col = 1; col < w - 1; col += 2)
                    {
                        int set = cellToSet[col];
                        if (rand.Next(2) == 0)
                        {
                            grid[col, row + 1] = 1;
                            setDownCount[set]++;
                        }
                        else
                        {
                            grid[col, row + 1] = 0;
                        }
                    }

                    for (int col = 1; col < w - 1; col += 2)
                    {
                        int set = cellToSet[col];
                        if (setDownCount[set] == 0)
                        {
                            grid[col, row + 1] = 1;
                            setDownCount[set]++;
                        }
                    }

                    // Подготовить следующую строку
                    var newCellToSet = new Dictionary<int, int>();
                    for (int col = 1; col < w - 1; col += 2)
                        if (grid[col, row + 1] == 1)
                            newCellToSet[col] = cellToSet[col];

                    cellToSet = newCellToSet;
                }
                else
                {
                    // Последняя строка: соединить все разные множества
                    for (int col = 1; col < w - 3; col += 2)
                    {
                        int set1 = cellToSet[col];
                        int set2 = cellToSet[col + 2];
                        if (set1 != set2)
                        {
                            grid[col + 1, row] = 1;
                            MergeSets(cellToSet, set1, set2);
                        }
                    }
                }
            }

            grid[maze.Entrance.X, maze.Entrance.Y] = 1;
            grid[maze.Exit.X, maze.Exit.Y] = 1;
        }

        private void MergeSets(Dictionary<int, int> cellToSet, int oldSet, int newSet)
        {
            var keysToUpdate = cellToSet.Where(kvp => kvp.Value == oldSet).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToUpdate)
            {
                cellToSet[key] = newSet;
            }
        }

        public List<Maze> GetAllMazes() => mazes;

        public void SaveMaze(Maze maze)
        {
            mazes.Add(maze);
        }

        // Волновой алгоритм
        public List<Point> FindPathWave(Maze maze)
        {
            int w = maze.Width, h = maze.Height;
            var grid = maze.Grid;
            var start = maze.Entrance;
            var end = maze.Exit;

            var queue = new Queue<Point>();
            var visited = new bool[w, h];
            // Откуда пришли в каждую точку
            var parent = new Dictionary<Point, Point>();
            
            queue.Enqueue(start);
            visited[start.X, start.Y] = true;
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end)
                    return RestorePath(parent, start, end);
                
                foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    int nx = current.X + dx, ny = current.Y + dy;
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h && grid[nx, ny] == 1 && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        parent[new Point(nx, ny)] = current;
                        queue.Enqueue(new Point(nx, ny));
                    }
                }
            }
            return new List<Point>();
        }
        
        // Восстановление пути
        private List<Point> RestorePath(Dictionary<Point, Point> parent, Point start, Point end)
        {
            var path = new List<Point>();
            var current = end;
            while (current != start && parent.ContainsKey(current))
            {
                path.Add(current);
                current = parent[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        // Алгоритм правой руки
        public List<Point> FindPathRightHand(Maze maze)
        {
            int w = maze.Width, h = maze.Height;
            var grid = maze.Grid;
            var path = new List<Point>();
            var current = maze.Entrance;
            var end = maze.Exit;
            
            // Направления
            var directions = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
            int dirIndex = 0;
            
            var visited = new HashSet<Point> { current };
            path.Add(current);
            
            while (current != end && path.Count < w * h)
            {
                bool moved = false;
                
                // Пытаемся идти вперед, потом поворачиваем налево
                for (int i = 0; i < 4; i++)
                {
                    int tryDir = (dirIndex - i + 4) % 4; // сначала налево
                    var (dx, dy) = directions[tryDir];
                    int nx = current.X + dx, ny = current.Y + dy;
                    
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h && grid[nx, ny] == 1)
                    {
                        current = new Point(nx, ny);
                        if (!visited.Contains(current))
                        {
                            visited.Add(current);
                            path.Add(current);
                        }
                        dirIndex = tryDir;
                        moved = true;
                        break;
                    }
                }
                
                if (!moved) break;
            }
            
            return current == end ? path : new List<Point>();
        }

        
    }
}