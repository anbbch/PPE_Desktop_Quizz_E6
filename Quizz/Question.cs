using MySql.Data.MySqlClient;

namespace Quizz
{
    public class Question
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public string Reponse { get; set; }
        public string GoodAnswer { get; set; }
        public int TypeId { get; set; }
        public int QuestionnaireId { get; set; }

        private MySqlConnection connection = DBConnection.Instance().Connection;

        public void AddQuestion(string contenu, string reponse, string goodAnswer,int typeId, int questionnaireId)
        {
            string query = "INSERT INTO Questions (conteneu, reponse,goodAnswer, type_id, questionnaire_id) VALUES (@Contenu, @Reponse, @GoogAnswer, @TypeId, @QuestionnaireId)";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Contenu", contenu);
                cmd.Parameters.AddWithValue("@Reponse", reponse);
                cmd.Parameters.AddWithValue("@GoodAnswer", goodAnswer);
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                cmd.Parameters.AddWithValue("@QuestionnaireId", questionnaireId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
