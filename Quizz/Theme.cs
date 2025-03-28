using MySql.Data.MySqlClient;

namespace Quizz
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private MySqlConnection connection = DBConnection.Instance().Connection;

        public void AddTheme(string name)
        {
            string query = "INSERT INTO Thème (name) VALUES (@Name)";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
