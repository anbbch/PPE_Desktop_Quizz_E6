using System;
using System.Drawing;
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
            this.BackColor = Color.FromArgb(255, 250, 240); // beige clair

            loginController = new LoginController();

            // Appliquer le style aux composants
            CustomizeUI();
        }

        private void CustomizeUI()
        {
            // Titre
            //lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            //lblTitle.ForeColor = Color.FromArgb(80, 50, 40); // marron foncé doux
            //lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Labels
            lblUsername.ForeColor = lblPassword.ForeColor = Color.FromArgb(100, 60, 50);
            lblUsername.Font = lblPassword.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Textboxes
            txtUsername.BackColor = txtPassword.BackColor = Color.White;
            txtUsername.ForeColor = txtPassword.ForeColor = Color.Black;
            txtUsername.BorderStyle = txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '*';

            // Bouton Login
            btnLogin.BackColor = Color.FromArgb(185, 145, 120); // beige soutenu
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (loginController.VerifyLogin(username, password))
            {
                SessionManager.Username = username;
                //MessageBox.Show($"Bienvenue, {username} !");
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
                        string email = PromptForInput("Entrez votre adresse email :", "Créer un compte");
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            try
                            {
                                loginController.CreateUser(name, username, password, email);
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
        }

        private string PromptForInput(string prompt, string title)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.Text = title;
                inputForm.BackColor = Color.FromArgb(255, 250, 240);

                Label lblPrompt = new Label() { Left = 20, Top = 20, Text = prompt, AutoSize = true };
                TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340 };
                //Button btnOk = new Button()
                //{
                //    Text = "OK",
                //    Left = 260,
                //    Top = 80,
                //    DialogResult = DialogResult.OK,
                //    BackColor = Color.FromArgb(180, 140, 110),
                //    ForeColor = Color.White,
                //    FlatStyle = FlatStyle.Flat
                //};
                Button btnCancel = new Button()
                {
                    Text = "Annuler",
                    Left = 180,
                    Top = 80,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                inputForm.Controls.Add(lblPrompt);
                inputForm.Controls.Add(txtInput);
                //inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                //inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                return inputForm.ShowDialog() == DialogResult.OK ? txtInput.Text : null;
            }
        }
    }
}
