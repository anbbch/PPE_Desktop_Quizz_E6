using System;
using System.Windows.Forms;
using Quizz.Model;

namespace Quizz.Controller
{
    public class LoginController
    {
        private User user;

        public LoginController()
        {
            user = new User();
        }

        public bool VerifyLogin(string username, string password)
        {
            return user.VerifyLogin(username, password);
        }

        public void CreateUser(string name, string username, string password, string email)
        {
            User user = new User();
            user.AddUser(name, "User", username, password, email);
        }

    }
}
