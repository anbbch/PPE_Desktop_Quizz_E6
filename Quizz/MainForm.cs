using MySql.Data.MySqlClient;
using Quizz.Controller;
using System;
using System.Data;
using System.Windows.Forms;

namespace Quizz
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            lblWelcome.Text = $"Bienvenue, {SessionManager.Username} ({SessionManager.Status})";
            AddCreateQuizButton();
            // Charger les thèmes et créer des boutons pour chaque thème
            LoadThemes();
        }

        // Charger les thèmes depuis la base de données
        private void LoadThemes()
        {
            this.Text = "Home";
            string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";// Chaîne de connexion

            // Créer et configurer le DataGridView
            DataGridView dgvThemes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };
            dgvThemes.CellDoubleClick += DgvThemes_CellDoubleClick; // Événement double-clic
            dgvThemes.MouseClick += DgvThemes_MouseClick; // Événement clic droit
            Controls.Add(dgvThemes);

            // Charger les données dans le DataGridView
            string query = "SELECT id, name FROM Thème"; // Requête pour récupérer les thèmes
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable themesTable = new DataTable();
                    adapter.Fill(themesTable);
                    dgvThemes.DataSource = null;  // Désactive la liaison
                    dgvThemes.DataSource = themesTable; // Lier les données
                    dgvThemes.Refresh();
                }
            }
        }

        private void DgvThemes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                int themeId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["id"].Value); // Récupérer l'ID du thème
                QuizForm quizForm = new QuizForm(themeId);
                quizForm.Show();
            }
        }

        private void DgvThemes_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView dgv = (DataGridView)sender;
                var hitTestInfo = dgv.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    dgv.ClearSelection();
                    dgv.Rows[hitTestInfo.RowIndex].Selected = true; // Sélectionner la ligne
                    ContextMenuStrip contextMenu = new ContextMenuStrip();

                    ToolStripMenuItem editItem = new ToolStripMenuItem("Edit");
                    editItem.Click += (s, ev) => EditTheme(dgv.Rows[hitTestInfo.RowIndex].Cells["id"].Value);

                    ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete");
                    deleteItem.Click += (s, ev) => DeleteTheme(dgv.Rows[hitTestInfo.RowIndex].Cells["id"].Value);

                    ToolStripMenuItem addItem = new ToolStripMenuItem("Add");
                    addItem.Click += (s, ev) => AddTheme();

                    contextMenu.Items.AddRange(new ToolStripMenuItem[] { editItem, deleteItem, addItem });
                    contextMenu.Show(dgv, new System.Drawing.Point(e.X, e.Y));
                }
            }
        }

        private void EditTheme(object themeId)
        {
            int id = Convert.ToInt32(themeId);
            string newName = Prompt.ShowDialog("Modifier le nom du thème :", "Édition du Thème");

            if (!string.IsNullOrEmpty(newName))
            {
                string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
                string query = "UPDATE Thème SET name = @Name WHERE id = @Id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newName);
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Thème modifié avec succès !");
                LoadThemes(); // Recharger les thèmes
            }
        }

        private void DeleteTheme(object themeId)
        {
            int id = Convert.ToInt32(themeId);
            // Code pour supprimer un thème avec l'ID spécifié
            string query = "DELETE FROM Thème WHERE id = @Id";

            string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Thème supprimé avec succès !");
            LoadThemes(); // Recharger les thèmes
        }

        private void AddTheme()
        {
            string themeName = Prompt.ShowDialog("Entrez le nom du nouveau thème :", "Ajout de Thème");

            if (!string.IsNullOrEmpty(themeName))
            {
                string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
                string query = "INSERT INTO Thème (name) VALUES (@Name)";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", themeName);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Thème ajouté avec succès !");
                LoadThemes(); // Recharger les thèmes
            }
        }

        private void AddCreateQuizButton()
        {
            Button btnCreateQuiz = new Button
            {
                Text = "Nouveau Questionnaire",
                Location = new System.Drawing.Point(10, 170), // Ajuster selon votre mise en page
                Size = new System.Drawing.Size(200, 40)
            };
            btnCreateQuiz.Click += BtnCreateQuiz_Click;
            this.Controls.Add(btnCreateQuiz);
        }

        private void BtnCreateQuiz_Click(object sender, EventArgs e)
        {
            CreateQuiz createQuizForm = new CreateQuiz();
            createQuizForm.ShowDialog();
        }


        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.ClearSession();
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
//Utiliser des bidinglist ou une méthode de rafraichissement