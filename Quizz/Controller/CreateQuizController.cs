using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;


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
                    string query = "SELECT name FROM thème";
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

        public void SaveQuiz(List<TextBox> questionInputs, List<Panel> answerPanels, List<ComboBox> typeInputs,ComboBox cbTheme, string questionnaireName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 1. Récupération du thème sélectionné
                    string themeName = cbTheme.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(themeName))
                    {
                        MessageBox.Show("Veuillez sélectionner un thème.");
                        return;
                    }

                    // 2. Récupérer l'ID du thème
                    int themeId;
                    string getThemeIdQuery = "SELECT id FROM thème WHERE name = @themeName";
                    using (MySqlCommand themeCmd = new MySqlCommand(getThemeIdQuery, connection))
                    {
                        themeCmd.Parameters.AddWithValue("@themeName", themeName);
                        object SResult = themeCmd.ExecuteScalar();
                        if (SResult == null)
                        {
                            MessageBox.Show("Thème non trouvé.");
                            return;
                        }
                        themeId = Convert.ToInt32(SResult);
                    }

                    // Insérer le questionnaire une seule fois
                    int questionnaireId;
                    string insertQuestionnaire = "INSERT INTO questionnaire (libelle, theme_id) VALUES (@libelle, @themeId)";
                    using (MySqlCommand insertQ = new MySqlCommand(insertQuestionnaire, connection))
                    {
                        insertQ.Parameters.AddWithValue("@libelle", questionnaireName);
                        insertQ.Parameters.AddWithValue("@themeId", themeId);
                        insertQ.ExecuteNonQuery();
                        questionnaireId = (int)insertQ.LastInsertedId;
                    }

                    // 4. Insérer les questions
                    for (int i = 0; i < questionInputs.Count; i++)
                    {
                        string questionText = questionInputs[i].Text;
                        if (!string.IsNullOrWhiteSpace(questionText))
                        {
                            // 4.1. Récupérer les réponses possibles (pour le cas QCM)
                            string answersText = string.Join(";", GetAnswers(answerPanels[i]));

                            // 4.2. Récupérer le type de question (1: Oui/Non, 2: QCM)
                            ComboBox cbType = typeInputs[i];
                            //ComboBox cbType = (ComboBox)answerPanels[i].Controls[0];  // On suppose que le ComboBox est le premier contrôle du Panel
                            int typeId = cbType.SelectedItem.ToString() == "Oui/Non" ? 1 : 2;

                            // 4.3. Définir la réponse correcte (ici, tu peux ajouter ta logique pour récupérer la bonne réponse)
                            string goodAnswer = GetGoodAnswer(answerPanels[i]);

                            // 4.4. Requête d'insertion de la question
                            string insertQuestion = @"INSERT INTO questions (conteneu, questionnaire_id, type_id, reponse, goodAnswer, valeur_multiple_id) 
                                              VALUES (@conteneu, @questionnaireId, @typeId, @reponse, @goodAnswer, NULL)";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuestion, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@conteneu", questionText);
                                insertCmd.Parameters.AddWithValue("@questionnaireId", questionnaireId);
                                insertCmd.Parameters.AddWithValue("@typeId", typeId);
                                insertCmd.Parameters.AddWithValue("@reponse", answersText);
                                insertCmd.Parameters.AddWithValue("@goodAnswer", goodAnswer);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    DialogResult result = MessageBox.Show("Quiz et questions enregistrés avec succès !\nSouhaitez-vous créer le procès-verbal du questionnaire ?", "Confirmation", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        GenerateRecapPdf(questionnaireName, themeName, questionInputs, answerPanels, typeInputs);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}");
            }
        }



        private List<string> GetAnswers(Panel answerPanel)
        {
            List<string> answers = new List<string>();

            foreach (Control control in answerPanel.Controls)
            {
                if (control is TextBox answerTextBox)
                {
                    string answerText = answerTextBox.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(answerText))
                    {
                        answers.Add(answerText);
                    }
                }
                else if (control is RadioButton radioButton)
                {
                    answers.Add(radioButton.Text.Trim());
                }
            }

            return answers;
        }


        private string GetGoodAnswer(Panel answerPanel)
        {
            // Cas QCM : TextBox + CheckBox
            bool isQCM = false;
            foreach (Control ctrl in answerPanel.Controls)
            {
                if (ctrl is CheckBox)
                {
                    isQCM = true;
                    break;
                }
            }

            if (isQCM)
            {
                List<string> goodAnswers = new List<string>();

                for (int i = 0; i < answerPanel.Controls.Count - 1; i += 2) // TextBox suivi d’une CheckBox
                {
                    if (answerPanel.Controls[i] is TextBox answerTextBox &&
                        answerPanel.Controls[i + 1] is CheckBox checkBox)
                    {
                        if (checkBox.Checked)
                        {
                            goodAnswers.Add(answerTextBox.Text.Trim());
                        }
                    }
                }

                return string.Join(";", goodAnswers); // Plusieurs bonnes réponses possibles
            }
            else
            {
                // Cas Oui/Non : un seul RadioButton coché
                foreach (Control ctrl in answerPanel.Controls)
                {
                    if (ctrl is RadioButton radioButton && radioButton.Checked)
                    {
                        return radioButton.Text.Trim(); // Une seule bonne réponse
                    }
                }

                return string.Empty;
            }
        }


        private void GenerateRecapPdf(string questionnaireName, string themeName, List<TextBox> questionInputs, List<Panel> answerPanels, List<ComboBox> typeInputs)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Procès-verbal du questionnaire";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont titleFont = new XFont("Verdana", 16, XFontStyleEx.Bold);
                XFont headerFont = new XFont("Verdana", 12, XFontStyleEx.Bold);
                XFont normalFont = new XFont("Verdana", 10);

                double y = 30;

                // Logo fictif en haut à gauche
                gfx.DrawString("BelleTable", titleFont, XBrushes.DarkBlue, new XRect(20, 10, page.Width, 20), XStringFormats.TopLeft);

                // Date en haut à droite
                gfx.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), normalFont, XBrushes.Black, new XRect(0, 10, page.Width - 20, 20), XStringFormats.TopRight);

                // Titre centré
                gfx.DrawString("Procès-verbal du questionnaire", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 40), XStringFormats.TopCenter);
                y += 40;

                // Infos de base
                gfx.DrawString($"Thème : {themeName}", headerFont, XBrushes.Black, new XRect(20, y, page.Width, 20), XStringFormats.TopLeft);
                y += 20;
                gfx.DrawString($"Nom du questionnaire : {questionnaireName}", headerFont, XBrushes.Black, new XRect(20, y, page.Width, 20), XStringFormats.TopLeft);
                y += 30;

                for (int i = 0; i < questionInputs.Count; i++)
                {
                    string questionText = questionInputs[i].Text;
                    string answers = string.Join(" ; ", GetAnswers(answerPanels[i]));
                    string goodAnswer = GetGoodAnswer(answerPanels[i]);

                    gfx.DrawString($"Q{i + 1}. {questionText}", normalFont, XBrushes.Black, new XRect(30, y, page.Width - 60, 20), XStringFormats.TopLeft);
                    y += 15;
                    gfx.DrawString($"Réponses : {answers}", normalFont, XBrushes.Black, new XRect(40, y, page.Width - 60, 20), XStringFormats.TopLeft);
                    y += 15;
                    gfx.DrawString($"Bonne réponse : {goodAnswer}", normalFont, XBrushes.Green, new XRect(40, y, page.Width - 60, 20), XStringFormats.TopLeft);
                    y += 25;

                    // Sauter de page si besoin
                    if (y > page.Height - 60)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 30;
                    }
                }

                string fileName = $"PV_{questionnaireName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                document.Save(outputPath);
                MessageBox.Show($"Procès-verbal généré avec succès sur le bureau :\n{fileName}");
                PdfFileUtility.ShowDocument(outputPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la génération du PDF : " + ex.Message);
            }
        }



        //        // Cette méthode est responsable de récupérer toutes les réponses possibles dans le Panel.
        //        private List<string> GetAnswers(Panel answerPanel)
        //        {
        //            List<string> answers = new List<string>();

        //            foreach (Control control in answerPanel.Controls)
        //            {
        //                if (control is TextBox answerTextBox)
        //                {
        //                    string answerText = answerTextBox.Text.Trim();
        //                    if (!string.IsNullOrWhiteSpace(answerText))
        //                    {
        //                        answers.Add(answerText);
        //                    }
        //                    else if (control is RadioButton radioButton)
        //                    {
        //                        answers.Add(radioButton.Text.Trim());
        //                    }
        //                }
        //            }

        //            return answers;
        //        }

        //        // Cette méthode permet de récupérer la bonne réponse parmi les réponses.
        //        private string GetGoodAnswer(Panel answerPanel)
        //{
        //            List<string> goodAnswers = new List<string>();

        //            for (int i = 0; i < answerPanel.Controls.Count; i += 2) // Un TextBox suivi d'une CheckBox
        //            {
        //                if (answerPanel.Controls[i] is TextBox answerTextBox && 
        //                    answerPanel.Controls[i + 1] is CheckBox checkBox)
        //                {
        //                    if (checkBox.Checked) // Si la case est cochée, c'est une bonne réponse
        //                    {
        //                        goodAnswers.Add(answerTextBox.Text.Trim());
        //                    }
        //                }
        //            }

        //            return string.Join(";", goodAnswers); // Retourne toutes les bonnes réponses séparées par ";"
        //        }
    }
}
