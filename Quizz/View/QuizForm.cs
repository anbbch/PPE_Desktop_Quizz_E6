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
                        AutoSize = true
                    };

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


        //// Gère l'événement de clic sur une réponse
        //private void btnAnswer_Click(object sender, EventArgs e)
        //{
        //    Button clickedButton = (Button)sender;
        //    string selectedAnswer = clickedButton.Tag.ToString(); // Récupère la réponse sélectionnée
        //    var currentQuestion = questions[currentQuestionIndex];

        //    if (!string.IsNullOrEmpty(currentQuestion.GoodAnswer) && currentQuestion.GoodAnswer.Split(';').Contains(selectedAnswer))
        //    {
        //        score++;
        //        MessageBox.Show("Bonne réponse !", "Correct", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    else
        //    {
        //        MessageBox.Show($"Mauvaise réponse. La bonne réponse était : {currentQuestion.GoodAnswer ?? "Inconnue"}", "Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }

        //    // Passe à la question suivante
        //    currentQuestionIndex++;
        //    ShowQuestion();
        //}


















        //// Affiche la question actuelle
        //private void ShowQuestion()
        //{
        //    if (currentQuestionIndex < questions.Count)
        //    {
        //        var question = questions[currentQuestionIndex];
        //        lblQuestion.Text = question.Contenu; // Affiche la question

        //        // Supprime les anciens boutons de réponse
        //        panelAnswers.Controls.Clear();

        //        // Génère les boutons pour les réponses
        //        var possibleAnswers = GetPossibleAnswers(question);
        //        int yOffset = 0;

        //        foreach (var answer in possibleAnswers)
        //        {
        //            Button answerButton = new Button
        //            {
        //                Text = answer,
        //                Tag = answer, // Associe la réponse au bouton via Tag
        //                Location = new System.Drawing.Point(10, yOffset),
        //                AutoSize = true
        //            };

        //            answerButton.Click += btnAnswer_Click; // Associe l'événement de clic
        //            panelAnswers.Controls.Add(answerButton);
        //            yOffset += 40; // Décalage vertical
        //        }
        //    }
        //    else
        //    {
        //        // Quiz terminé
        //        MessageBox.Show($"Quiz terminé ! Votre score : {score}/{questions.Count}", "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        this.Close();
        //    }
        //}

        //// Génère des réponses possibles pour la question actuelle
        //private List<string> GetPossibleAnswers(Question question)
        //{
        //    List<string> answers = new List<string> { question.Reponse };

        //    // Ajoute des réponses fictives
        //    while (answers.Count < 4)
        //    {
        //        string fakeAnswer = "Fausse réponse " + answers.Count;
        //        if (!answers.Contains(fakeAnswer))
        //        {
        //            answers.Add(fakeAnswer);
        //        }
        //    }

        //    // Mélange les réponses pour aléatoirement les disposer
        //    return answers.OrderBy(a => Guid.NewGuid()).ToList();
        //}

        //// Gère l'événement de clic sur une réponse
        //private void btnAnswer_Click(object sender, EventArgs e)
        //{
        //    Button clickedButton = (Button)sender;
        //    string selectedAnswer = clickedButton.Tag.ToString(); // Récupère la réponse sélectionnée
        //    var currentQuestion = questions[currentQuestionIndex];

        //    // Vérifie si la réponse est correcte
        //    if (selectedAnswer == currentQuestion.Reponse)
        //    {
        //        score++;
        //        MessageBox.Show("Bonne réponse !", "Correct", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    else
        //    {
        //        MessageBox.Show($"Mauvaise réponse. La bonne réponse était : {currentQuestion.Reponse}", "Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }

        //    // Passe à la question suivante
        //    currentQuestionIndex++;
        //    ShowQuestion();
        //}
    }
}