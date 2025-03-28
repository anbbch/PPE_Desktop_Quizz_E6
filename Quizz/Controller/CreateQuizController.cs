using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Quizz.Controller
{
    public class CreateQuizController
    {
        private readonly string connectionString;

        public CreateQuizController()
        {
            connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
        }
        public void LoadThemes(ComboBox cbTheme)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT name FROM Thème";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string themeName = reader.IsDBNull(reader.GetOrdinal("name")) ? "Nom indisponible" : reader.GetString("name");
                            cbTheme.Items.Add(themeName);

                            //cbTheme.Items.Add(reader.GetString("nom"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des thèmes: {ex.Message}");
            }
        }

        public void SaveQuiz(List<TextBox> questionInputs, List<Panel> answerPanels, ComboBox cbTheme)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var question in questionInputs)
                    {
                        if (!string.IsNullOrWhiteSpace(question.Text))
                        {
                            string theme = cbTheme.SelectedItem.ToString();
                            string query = "INSERT INTO questions (theme, question) VALUES (@theme, @question)";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@theme", theme);
                                cmd.Parameters.AddWithValue("@question", question.Text);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Quiz enregistré avec succès!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}");
            }
        }
    }
}
