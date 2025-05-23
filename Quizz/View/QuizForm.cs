using MySql.Data.MySqlClient;
using Quizz.Controller;
using Quizz.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quizz
{
    public partial class QuizForm : Form
    {
        private QuizController quizController;

        public QuizForm(int themeId)
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(255, 245, 225); // fond beige clair
            this.Font = new Font("Segoe UI", 11); // police agréable et moderne

            lblQuestion.ForeColor = Color.FromArgb(101, 67, 33); // texte marron
            lblQuestion.Font = new Font("Segoe UI", 13, FontStyle.Bold);

            panelAnswers.BackColor = Color.FromArgb(255, 245, 225); // fond des réponses en blanc cassé
            panelAnswers.BorderStyle = BorderStyle.None;

            quizController = new QuizController(themeId);
            ShowQuestion();
        }

        // Charge les questions depuis la base de données
        private void ShowQuestion()
        {
            var question = quizController.GetNextQuestion();

            if (question != null)
            {
                lblQuestion.Text = question.Contenu;
                panelAnswers.Controls.Clear();

                var possibleAnswers = quizController.GetPossibleAnswers(question);
                int yOffset = 0;

                foreach (var answer in possibleAnswers)
                {
                    Button answerButton = new Button
                    {
                        Text = answer,
                        Tag = answer,
                        Location = new System.Drawing.Point(10, yOffset),
                        Width = 400,
                        Height = 35,
                        BackColor = Color.FromArgb(101, 67, 33), // fond marron
                        ForeColor = Color.White,                // texte blanc
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    };
                    answerButton.FlatAppearance.BorderSize = 0;


                    answerButton.Click += (s, e) => HandleAnswerClick(question, answer);
                    panelAnswers.Controls.Add(answerButton);
                    yOffset += 40;
                }
            }
            else
            {
                MessageBox.Show($"Quiz terminé ! Votre score : {quizController.GetScore()}/{quizController.GetQuestions().Count}",
                    "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        // Génère des réponses possibles pour la question actuelle
        private List<string> GetPossibleAnswers(Question question)
        {
            // Sépare les réponses possibles avec ";"
            List<string> answers = new List<string>();

            if (question.TypeId == 1)  // Si c'est une question de type Oui/Non
            {
                answers.AddRange(question.Reponse.Split(';'));
                //answers.Add("Oui");
                //answers.Add("Non");
            }
            else if (question.TypeId == 3)  // Si c'est une question de type QCM avec réponses multiples
            {
                // Sépare les réponses possibles avec ";"
                answers.AddRange(question.Reponse.Split(';'));
            }

            // Ajoute des réponses fictives si nécessaire (pour type_id == 3)
            while (answers.Count < 2)
            {
                string fakeAnswer = "Aucune des réponses ";
                if (!answers.Contains(fakeAnswer))
                {
                    answers.Add(fakeAnswer);
                }
            }

            // Mélange les réponses pour aléatoirement les disposer
            return answers.OrderBy(a => Guid.NewGuid()).ToList();
        }


        private void HandleAnswerClick(Question question, string selectedAnswer)
        {
            if (quizController.CheckAnswer(question, selectedAnswer))
            {
                MessageBox.Show("Bonne réponse !", "Correct", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Mauvaise réponse. La bonne réponse était : {question.GoodAnswer ?? "Inconnue"}",
                    "Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            quizController.IncrementQuestionIndex();
            ShowQuestion();
        }

    }
}