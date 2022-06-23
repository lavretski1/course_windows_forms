using ApplicationProject.DAL;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Text;

namespace ApplicationProject
{
    internal static class Program
    {
        private static RentContext? context = null;
        private static string? username = null;
        private static bool tryAgain = true;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            while (tryAgain)
            {
                var form = new LoginForm();
                form.FormClosing += LogIn;
                Application.Run(form);
            }

            if (context is null) 
            {
                return;
            }

            Application.Run(new MainWindow(context, IsAdmin(username)));
        }

        private static void LogIn(object? form, FormClosingEventArgs args) 
        {
            var loginForm = form as LoginForm;

            if (loginForm.Username is null) 
            {
                if (loginForm.Register == true) 
                {
                    new RegisterForm().ShowDialog();
                    return;
                }

                tryAgain = false;
                return;
            }

            string template = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(template, loginForm.Username, loginForm.Password);

            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();

            builder.UseSqlServer(stringBuilder.ToString());

            RentContext context = new RentContext(builder.Options);

            try
            {
                context.Database.OpenConnection();
                context.Database.CloseConnection();
            }
            catch(Exception e)
            {
                if (e.InnerException is null)
                {
                    MessageBox.Show($"Unable to connect to database \nDetails: {e.Message}");
                    return;
                }
                MessageBox.Show($"Unable to connect to database \nDetails: {e.InnerException.Message.Substring(0, e.InnerException.Message.IndexOf('('))}");
                return;
            }

            Program.context = context;
            Program.username = loginForm.Username;
            tryAgain = false;
        }

        private static bool IsAdmin(string username) 
        {
            return context.Admins.Any(a => a.Username == username);
        }
    }
}