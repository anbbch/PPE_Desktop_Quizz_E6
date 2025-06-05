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
        private List<Question> questions; // Liste des questions
        private int currentQuestionIndex = 0; // Index de la question actuelle
        private int score = 0; // Score de l'utilisateur
        private int themeId; // ID du thème sélectionné

        public QuizForm(int themeId)
        {
            InitializeComponent();
            this.themeId = themeId; // Récupère l'ID du thème
            LoadQuestions();
            ShowQuestion();
        }

        // Charge les questions depuis la base de données
        private void LoadQuestions()
        {
            this.Text = "Quizz";
            questions = new List<Question>();

            DBConnection DBCon = new DBConnection
            {
                Server = "localhost",
                DatabaseName = "quizz",
                UserName = "root",
                Password = Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==")
            };

            if (DBCon.IsConnect())
            {
                string query = "SELECT id, conteneu, reponse, goodAnswer, type_id FROM questions WHERE questionnaire_id = @ThemeId";
                using (var cmd = new MySqlCommand(query, DBCon.Connection))
                {
                    cmd.Parameters.AddWithValue("@ThemeId", themeId); // Filtrage par thème
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetInt32("id"),
                                Contenu = reader.GetString("conteneu"),
                                Reponse = reader.GetString("reponse"),
                                GoodAnswer = reader.GetString("goodAnswer"),
                                TypeId = reader.GetInt32("type_id")
                            });
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Échec de la connexion à la base de données.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ShowQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];
                lblQuestion.Text = question.Contenu; // Affiche la question

                // Supprime les anciens boutons de réponse
                panelAnswers.Controls.Clear();

                // Génère les boutons pour les réponses
                var possibleAnswers = GetPossibleAnswers(question);
                int yOffset = 0;

                foreach (var answer in possibleAnswers)
                {
                    Button answerButton = new Button
                    {
                        Text = answer,
                        Tag = answer, // Associe la réponse au bouton via Tag
                        Location = new System.Drawing.Point(10, yOffset),
                        AutoSize = true
                    };

                    answerButton.Click += btnAnswer_Click; // Associe l'événement de clic
                    panelAnswers.Controls.Add(answerButton);
                    yOffset += 40; // Décalage vertical
                }
            }
            else
            {
                // Quiz terminé
                MessageBox.Show($"Quiz terminé ! Votre score : {score}/{questions.Count}", "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Gère l'événement de clic sur une réponse
        private void btnAnswer_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string selectedAnswer = clickedButton.Tag.ToString(); // Récupère la réponse sélectionnée
            var currentQuestion = questions[currentQuestionIndex];

            if (!string.IsNullOrEmpty(currentQuestion.GoodAnswer) && currentQuestion.GoodAnswer.Split(';').Contains(selectedAnswer))
            {
                score++;
                MessageBox.Show("Bonne réponse !", "Correct", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Mauvaise réponse. La bonne réponse était : {currentQuestion.GoodAnswer ?? "Inconnue"}", "Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Passe à la question suivante
            currentQuestionIndex++;
            ShowQuestion();
        }


















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