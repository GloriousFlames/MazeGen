using MazeGen.Data;
using MazeGen.Services;

namespace MazeGen
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var db = new Database();
            var mazeService = new MazeService();
            Application.Run(new LoginForm(db, mazeService));
        }
    }
}