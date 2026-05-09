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
            //var loginForm = new LoginForm(db);
            //Application.Run(loginForm);
            //    Application.Run(new LoginForm());
            //    Application.Run(new RegisterForm());
            //    Application.Run(new AboutForm());
            //    Application.Run(new AdminMainForm(new Models.User(), mazeService));
            //    Application.Run(new LoadMazeForm());
            //   Application.Run(new PlayerMainForm(new Models.User()));
            //    Application.Run(new SaveMazeForm());
        }
    }
}