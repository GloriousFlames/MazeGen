using System;
using System.Windows.Forms;

namespace MazeGen
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new LoginForm());
        //    Application.Run(new RegisterForm());
        //    Application.Run(new AboutForm());
        //    Application.Run(new AdminMainForm(new Models.User()));
        //    Application.Run(new LoadMazeForm());
            Application.Run(new PlayerMainForm(new Models.User()));
        //    Application.Run(new SaveMazeForm());
        }
    }
}