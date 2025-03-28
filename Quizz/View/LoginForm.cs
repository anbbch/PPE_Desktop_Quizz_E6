using System;
using System.Windows.Forms;
using Quizz.Controller;
using Quizz.Model;

namespace Quizz
{
    public partial class LoginForm : Form
    {
        private LoginController loginController;

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Bienvenue !";
            loginController = new LoginController();
            txtPassword.PasswordChar = '*'; // Utiliser 'µ' pour masquer le mot de passe
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (loginController.VerifyLogin(username, password))
            {
                SessionManager.Username = username;
                MessageBox.Show($"Bienvenue, {username} !");
                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            else
            {
                var result = MessageBox.Show(
                    "Nom d'utilisateur ou mot de passe incorrect.\nSouhaitez-vous créer un compte avec ces informations ?",
                    "Créer un compte",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    string name = PromptForInput("Entrez votre nom :", "Créer un compte");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        try
                        {
                            loginController.CreateUser(name, username, password);
                            MessageBox.Show("Compte créé avec succès ! Veuillez vous reconnecter.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erreur lors de la création du compte : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private string PromptForInput(string prompt, string title)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.Text = title;

                Label lblPrompt = new Label() { Left = 20, Top = 20, Text = prompt, AutoSize = true };
                TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340 };
                Button btnOk = new Button() { Text = "OK", Left = 260, Top = 80, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Annuler", Left = 180, Top = 80, DialogResult = DialogResult.Cancel };

                inputForm.Controls.Add(lblPrompt);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                return inputForm.ShowDialog() == DialogResult.OK ? txtInput.Text : null;
            }
        }
    }
}
