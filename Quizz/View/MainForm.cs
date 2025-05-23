using MySql.Data.MySqlClient;
using Quizz.Controller;
using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace Quizz
{
    public partial class MainForm : Form
    {
        private readonly MainFormController controller;
        private DataGridView dgvThemes;
        private Button btnCreateQuiz;

        public MainForm()
        {
            InitializeComponent();
            controller = new MainFormController();
            lblWelcome.Text = $"Bienvenue, {SessionManager.Username} ({SessionManager.Status})";
            LoadThemes();
        }

        private void LoadThemes()
        {
            this.Text = "Home";
            Controls.Clear(); // Nettoyer les anciens contrôles si on recharge

            // Créer un panel pour contenir le DataGridView
            Panel panelThemes = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Top,
                Height = 300 // hauteur fixe ou ajustable dynamiquement si besoin
            };
            panelThemes.BorderStyle= BorderStyle.None;

            dgvThemes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = System.Drawing.Color.FromArgb(255, 245, 225),
                GridColor = System.Drawing.Color.FromArgb(200, 150, 120),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(180, 140, 120),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.White,
                    ForeColor = System.Drawing.Color.FromArgb(75, 46, 46),
                    Font = new System.Drawing.Font("Segoe UI", 9)
                }
            };
            dgvThemes.BorderStyle = BorderStyle.None;

            dgvThemes.CellDoubleClick += DgvThemes_CellDoubleClick;
            dgvThemes.MouseClick += DgvThemes_MouseClick;

            panelThemes.Controls.Add(dgvThemes);
            Controls.Add(panelThemes);

            DataTable themesTable = controller.LoadThemes();
            dgvThemes.DataSource = themesTable;

            // Ajouter le bouton dynamiquement en dessous du panel
            AddCreateQuizButton(panelThemes.Bottom + 5);

            this.BackColor = System.Drawing.Color.FromArgb(255, 245, 225); // crème

        }


        private void DgvThemes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                object quizIdObj = dgv.Rows[e.RowIndex].Cells["quiz_id"].Value;
                if (quizIdObj != DBNull.Value)
                {
                    int quizId = Convert.ToInt32(quizIdObj);
                    QuizForm quizForm = new QuizForm(quizId); // à adapter si besoin
                    quizForm.Show();
                }
                else
                {
                    MessageBox.Show("Ce thème ne contient aucun questionnaire.");
                }
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
                    dgv.Rows[hitTestInfo.RowIndex].Selected = true;
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
                controller.EditTheme(id, newName);
                MessageBox.Show("Thème modifié avec succès !");
                LoadThemes();
            }
        }

        private void DeleteTheme(object themeId)
        {
            int id = Convert.ToInt32(themeId);
            controller.DeleteTheme(id);
            MessageBox.Show("Thème supprimé avec succès !");
            LoadThemes();
        }

        private void AddTheme()
        {
            string themeName = Prompt.ShowDialog("Entrez le nom du nouveau thème :", "Ajout de Thème");
            if (!string.IsNullOrEmpty(themeName))
            {
                controller.AddTheme(themeName);
                MessageBox.Show("Thème ajouté avec succès !");
                LoadThemes();
            }
        }

        private void AddCreateQuizButton(int topPosition)
        {
            btnCreateQuiz = new Button
            {
                Text = "Nouveau Questionnaire",
                Location = new System.Drawing.Point(10, topPosition),
                Size = new System.Drawing.Size(200, 40),
                BackColor = System.Drawing.Color.FromArgb(169, 116, 110),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            btnCreateQuiz.Click += BtnCreateQuiz_Click;
            Controls.Add(btnCreateQuiz);
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
