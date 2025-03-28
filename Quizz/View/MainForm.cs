using MySql.Data.MySqlClient;
using Quizz.Controller;
using System;
using System.Data;
using System.Windows.Forms;

namespace Quizz
{
    public partial class MainForm : Form
    {
        private readonly MainFormController controller;

        public MainForm()
        {
            InitializeComponent();
            controller = new MainFormController();
            lblWelcome.Text = $"Bienvenue, {SessionManager.Username} ({SessionManager.Status})";
            AddCreateQuizButton();
            LoadThemes();
        }

        private void LoadThemes()
        {
            this.Text = "Home";
            DataGridView dgvThemes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };
            dgvThemes.CellDoubleClick += DgvThemes_CellDoubleClick;
            dgvThemes.MouseClick += DgvThemes_MouseClick;
            Controls.Add(dgvThemes);

            DataTable themesTable = controller.LoadThemes();
            dgvThemes.DataSource = themesTable;
        }

        private void DgvThemes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                int themeId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["id"].Value);
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

        private void AddCreateQuizButton()
        {
            Button btnCreateQuiz = new Button
            {
                Text = "Nouveau Questionnaire",
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(200, 40)
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
