using System;
using System.Windows.Forms;

namespace Quizz
{
    public partial class LoginForm : Form
    {
        private User user;

        public LoginForm()
        {
            InitializeComponent();
            user = new User();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (user.VerifyLogin(username, password))
            {
                // R�cup�ration des informations utilisateur
                SessionManager.Username = username;
                SessionManager.Status = user.Status;

                MessageBox.Show($"Bienvenue, {username} !");
                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            else
            {
                // Demander si l'utilisateur souhaite cr�er un compte
                var result = MessageBox.Show(
                    "Nom d'utilisateur ou mot de passe incorrect.\nSouhaitez-vous cr�er un compte avec ces informations ?",
                    "Cr�er un compte",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    string name = PromptForInput("Entrez votre nom :", "Cr�er un compte");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        string status = "User"; // Par d�faut, nouveau compte = utilisateur standard
                        try
                        {
                            user.AddUser(name, status, username, password);
                            MessageBox.Show("Compte cr�� avec succ�s ! Veuillez vous reconnecter.", "Succ�s", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erreur lors de la cr�ation du compte : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // M�thode pour afficher une bo�te de dialogue et demander une saisie utilisateur
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

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    return txtInput.Text;
                }
                else
                {
                    return null;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e) { }
    }
}
