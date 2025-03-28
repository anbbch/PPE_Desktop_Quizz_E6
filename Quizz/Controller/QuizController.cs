using MySql.Data.MySqlClient;
using Quizz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Quizz.Controller
{
    public class QuizController
    {
        private List<Question> questions;
        private int currentQuestionIndex;
        private int score;

        public QuizController(int themeId)
        {
            currentQuestionIndex = 0;
            score = 0;
            LoadQuestions(themeId);
        }

        public List<Question> GetQuestions()
        {
            return questions;
        }

        public int GetScore()
        {
            return score;
        }

        public void LoadQuestions(int themeId)
        {
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
                    cmd.Parameters.AddWithValue("@ThemeId", themeId);
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

        public Question GetNextQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                return questions[currentQuestionIndex];
            }
            return null;
        }

        public void IncrementQuestionIndex()
        {
            currentQuestionIndex++;
        }

        public bool CheckAnswer(Question question, string selectedAnswer)
        {
            if (!string.IsNullOrEmpty(question.GoodAnswer) && question.GoodAnswer.Split(';').Contains(selectedAnswer))
            {
                score++;
                return true;
            }
            return false;
        }

        public List<string> GetPossibleAnswers(Question question)
        {
            List<string> answers = new List<string>();

            if (question.TypeId == 1)
            {
                answers.AddRange(question.Reponse.Split(';'));
            }
            else if (question.TypeId == 3)
            {
                answers.AddRange(question.Reponse.Split(';'));
            }

            while (answers.Count < 2)
            {
                string fakeAnswer = "Aucune des réponses";
                if (!answers.Contains(fakeAnswer))
                {
                    answers.Add(fakeAnswer);
                }
            }

            return answers.OrderBy(a => Guid.NewGuid()).ToList();
        }
    }
}
