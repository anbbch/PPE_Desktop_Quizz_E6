using Quizz.Controller;
using Quizz.Model;

namespace Quizz
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DBConnection db = DBConnection.Instance();
            db.Server = "localhost";
            db.DatabaseName = "quizz";
            db.UserName = "root";
            db.Password = Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==");

            if (db.IsConnect())
            {
                Application.Run(new LoginForm());
                db.Close();
            }
            else
            {
                MessageBox.Show("Impossible de se connecter à la base de données.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //[STAThread]
        //static void Main()
        //{
        //    // To customize application configuration such as set high DPI settings or default font,
        //    // see https://aka.ms/applicationconfiguration.
        //    ApplicationConfiguration.Initialize();
        //    Application.Run(new LoginForm());
        //}
    }
}