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
        List<ComboBox> typeInputs = new List<ComboBox>(); 
        private readonly CreateQuizController controller;

        public CreateQuiz()
        {
            InitializeComponent();
            SetupForm();

            controller = new CreateQuizController();
            controller.LoadThemes(cbTheme);
        }

        private void SetupForm()
        {
            this.Text = "Créer un Quiz";
            this.Size = new Size(1000, 900);
            this.BackColor = Color.FromArgb(255, 250, 240); // Fond crème

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(255, 250, 240)
            };
            this.Controls.Add(mainPanel);

            Label lblTheme = new Label
            {
                Text = "Choisir un Thème :",
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(101, 67, 33) // Marron
            };
            mainPanel.Controls.Add(lblTheme);

            cbTheme = new ComboBox
            {
                Location = new Point(150, 20),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            mainPanel.Controls.Add(cbTheme);

            Label lblNomQuestionnaire = new Label
            {
                Text = "Nom du Questionnaire :",
                Location = new Point(470, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(101, 67, 33)
            };
            mainPanel.Controls.Add(lblNomQuestionnaire);

            TextBox txtNomQuestionnaire = new TextBox
            {
                Name = "txtNomQuestionnaire",
                Location = new Point(630, 20),
                Width = 300,
                BackColor = Color.White
            };
            mainPanel.Controls.Add(txtNomQuestionnaire);

            Panel questionsPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(940, 1800),
                BorderStyle = BorderStyle.None,
                AutoScroll = false,
                BackColor = Color.FromArgb(255, 250, 240)
            };
            mainPanel.Controls.Add(questionsPanel);

            for (int i = 0; i < 10; i++)
            {
                Panel panel = new Panel
                {
                    Size = new Size(900, 170),
                    Location = new Point(10, i * 180),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(245, 235, 220)
                };
                questionsPanel.Controls.Add(panel);

                Label lblQuestion = new Label
                {
                    Text = $"Question {i + 1}:",
                    Location = new Point(10, 10),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(101, 67, 33)
                };
                panel.Controls.Add(lblQuestion);

                TextBox txtQuestion = new TextBox
                {
                    Location = new Point(120, 10),
                    Width = 500,
                    BackColor = Color.White
                };
                panel.Controls.Add(txtQuestion);
                questionInputs.Add(txtQuestion);

                ComboBox cbType = new ComboBox
                {
                    Location = new Point(650, 10),
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BackColor = Color.White
                };
                cbType.Items.AddRange(new string[] { "Oui/Non", "QCM" });
                cbType.SelectedIndexChanged += (s, e) => ToggleAnswerOptions(panel, cbType.SelectedItem.ToString());
                panel.Controls.Add(cbType);
                typeInputs.Add(cbType);

                Panel answerPanel = new Panel
                {
                    Location = new Point(10, 50),
                    Size = new Size(880, 100),
                    AutoScroll = true,
                    BackColor = Color.FromArgb(255, 250, 240)
                };
                panel.Controls.Add(answerPanel);
                answerPanels.Add(answerPanel);
            }

            Button btnSave = new Button
            {
                Text = "Enregistrer",
                Size = new Size(150, 40),
                Location = new Point(400, 1875),
                BackColor = Color.FromArgb(150, 75, 0), // Marron
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => controller.SaveQuiz(questionInputs, answerPanels, typeInputs, cbTheme, txtNomQuestionnaire.Text);
            mainPanel.Controls.Add(btnSave);
        }




        private void ToggleAnswerOptions(Panel panel, string type)
        {
            Panel answerPanel = panel.Controls[panel.Controls.Count - 1] as Panel;
            answerPanel.Controls.Clear();

            if (type == "Oui/Non")
            {
                answerPanel.Controls.Add(new RadioButton { Text = "Oui", Location = new Point(10, 10) });
                answerPanel.Controls.Add(new RadioButton { Text = "Non", Location = new Point(150, 10) });
            }
            else if (type == "QCM")
            {
                for (int i = 0; i < 3; i++)
                {
                    int y = i * 30;
                    answerPanel.Controls.Add(new TextBox { Location = new Point(10, y), Width = 400 });
                    answerPanel.Controls.Add(new CheckBox { Location = new Point(420, y + 3) });
                }
            }

        }
    }
}
