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
            this.Size = new Size(1000, 800);

            Label lblTheme = new Label { Text = "Choisir un Thème :", Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTheme);

            cbTheme = new ComboBox { Location = new Point(150, 20), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cbTheme);

            Panel questionsPanel = new Panel { Location = new Point(20, 60), Size = new Size(1550, 700), AutoScroll = true };
            this.Controls.Add(questionsPanel);

            for (int i = 0; i < 10; i++)
            {
                Panel panel = new Panel { Size = new Size(1500, 150), Location = new Point(10, i * 160) };
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

                Panel answerPanel = new Panel { Location = new Point(10, 50), Size = new Size(1450, 100), AutoScroll = true };
                panel.Controls.Add(answerPanel);
                answerPanels.Add(answerPanel);
            }

            Button btnSave = new Button { Text = "Enregistrer", Location = new Point(200, 850) };
            btnSave.Click += (s, e) => controller.SaveQuiz(questionInputs, answerPanels, cbTheme);
            questionsPanel.Controls.Add(btnSave);
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
                    answerPanel.Controls.Add(new TextBox { Location = new Point(10 + (i * 250), 10), Width = 200 });
                    answerPanel.Controls.Add(new CheckBox { Location = new Point(220 + (i * 250), 10) });
                }
            }
        }
    }
}
