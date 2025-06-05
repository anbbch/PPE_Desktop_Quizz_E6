using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Quizz.Controller;

namespace Quizz
{
    public partial class CreateQuiz : Form
    {
        private ComboBox cbTheme;
        private List<TextBox> questionInputs = new List<TextBox>();
        private List<Panel> answerPanels = new List<Panel>();

        public CreateQuiz()
        {
            InitializeComponent();
            SetupForm();
            LoadThemes();
        }

        private void SetupForm()
        {
            this.Text = "Créer un Quiz";
            this.Size = new Size(1000,800); // Agrandissement de la fenêtre

            Label lblTheme = new Label { Text = "Choisir un Thème :", Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTheme);

            cbTheme = new ComboBox { Location = new Point(150, 20), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cbTheme);

            Panel questionsPanel = new Panel { Location = new Point(20, 60), Size = new Size(1550, 700), AutoScroll = true }; // Agrandissement du panel
            this.Controls.Add(questionsPanel);

            for (int i = 0; i < 10; i++)
            {
                Panel panel = new Panel { Size = new Size(1500, 150), Location = new Point(10, i * 160) }; // Ajustement de la hauteur
                questionsPanel.Controls.Add(panel);

                Label lblQuestion = new Label { Text = $"Question {i + 1}:", Location = new Point(10, 10), AutoSize = true };
                panel.Controls.Add(lblQuestion);

                TextBox txtQuestion = new TextBox { Location = new Point(120, 10), Width = 500 };
                panel.Controls.Add(txtQuestion);
                questionInputs.Add(txtQuestion);

                ComboBox cbType = new ComboBox { Location = new Point(650, 10), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
                cbType.Items.AddRange(new string[] { "Oui/Non", "QCM" });
                cbType.SelectedIndexChanged += (s, e) => ToggleAnswerOptions(panel, cbType.SelectedItem.ToString());
                panel.Controls.Add(cbType);

                Panel answerPanel = new Panel { Location = new Point(10, 50), Size = new Size(1450, 100), AutoScroll = true }; // Augmentation de la taille du panel
                panel.Controls.Add(answerPanel);
                answerPanels.Add(answerPanel);
            }
            // Ajout du bouton "Enregistrer" dans le questionsPanel
            Button btnSave = new Button { Text = "Enregistrer", Location = new Point(200, 850), Enabled = true, Visible = true };
            btnSave.Click += SaveQuiz;
            questionsPanel.Controls.Add(btnSave);
        }

        private void ToggleAnswerOptions(Panel panel, string type)
        {
            Panel answerPanel = panel.Controls[panel.Controls.Count - 1] as Panel;
            answerPanel.Controls.Clear();

            if (type == "Oui/Non")
            {
                RadioButton rbYes = new RadioButton { Text = "Oui", Location = new Point(10, 10) };
                RadioButton rbNo = new RadioButton { Text = "Non", Location = new Point(150, 10) };
                answerPanel.Controls.Add(rbYes);
                answerPanel.Controls.Add(rbNo);
            }
            else if (type == "QCM")
            {
                int spacing = 250; // Espacement entre les options
                for (int i = 0; i < 3; i++)
                {
                    TextBox txtOption = new TextBox { Location = new Point(10 + (i * spacing), 10), Width = 200 };
                    CheckBox chkCorrect = new CheckBox { Location = new Point(220 + (i * spacing), 10) }; // Ajustement de l'alignement vertical
                    answerPanel.Controls.Add(txtOption);
                    answerPanel.Controls.Add(chkCorrect);
                }
            }
        }

        private void LoadThemes()
        {
            string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
            string query = "SELECT id, name FROM thème";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable themesTable = new DataTable();
                    adapter.Fill(themesTable);

                    cbTheme.DataSource = themesTable;
                    cbTheme.DisplayMember = "name";
                    cbTheme.ValueMember = "id";
                }
            }
        }

        private void SaveQuiz(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
            string themeId = cbTheme.SelectedValue.ToString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                for (int i = 0; i < questionInputs.Count; i++)
                {
                    string questionText = questionInputs[i].Text;
                    Panel answerPanel = answerPanels[i];
                    string answerType = (answerPanel.Parent.Controls[2] as ComboBox).SelectedItem?.ToString();
                    string answers = "";
                    string goodAnswer = "";
                    int typeId = answerType == "Oui/Non" ? 1 : 3;

                    if (answerType == "Oui/Non")
                    {
                        answers = "Oui;Non";
                        goodAnswer = (answerPanel.Controls[0] as RadioButton).Checked ? "Oui" : "Non";
                    }
                    else
                    {
                        List<string> options = new List<string>();
                        List<string> correctAnswers = new List<string>();
                        for (int j = 0; j < 3; j++)
                        {
                            TextBox txtOption = answerPanel.Controls[j * 2] as TextBox;
                            CheckBox chkCorrect = answerPanel.Controls[j * 2 + 1] as CheckBox;
                            if (!string.IsNullOrEmpty(txtOption.Text))
                            {
                                options.Add(txtOption.Text);
                                if (chkCorrect.Checked)
                                    correctAnswers.Add(txtOption.Text);
                            }
                        }
                        answers = string.Join(";", options);
                        goodAnswer = string.Join(";", correctAnswers);
                    }

                    string query = "INSERT INTO questions (contenu, reponse, goodAnswer, type_id, questionnaire_id) VALUES (@contenu, @reponse, @goodAnswer, @type_id, @questionnaire_id)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@contenu", questionText);
                        command.Parameters.AddWithValue("@reponse", answers);
                        command.Parameters.AddWithValue("@goodAnswer", goodAnswer);
                        command.Parameters.AddWithValue("@type_id", typeId);
                        command.Parameters.AddWithValue("@questionnaire_id", themeId);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Quiz enregistré avec succès !");
            }
        }
    }
}
