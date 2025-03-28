using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace Quizz
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        private MySqlConnection connection = DBConnection.Instance().Connection;

        // Méthode pour générer un salt
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Méthode pour hasher un mot de passe avec un salt
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Méthode pour ajouter un utilisateur dans la base
        public void AddUser(string name, string status, string username, string password)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            string query = "INSERT INTO Users (name, status, username, password, salt) VALUES (@Name, @Status, @Username, @Password, @Salt)";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Salt", salt);
                cmd.ExecuteNonQuery();
            }
        }

        // Méthode pour vérifier le login d'un utilisateur
        public bool VerifyLogin(string username, string password)
        {
            string query = "SELECT password, salt FROM Users WHERE username = @Username";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader.GetString("password");
                        string salt = reader.GetString("salt");

                        // Hasher le mot de passe fourni avec le salt stocké
                        string hashedPassword = HashPassword(password, salt);

                        // Vérifier si les deux mots de passe correspondent
                        return hashedPassword == storedPassword;
                    }
                }
            }
            return false; // Aucun utilisateur trouvé ou mot de passe incorrect
        }
    }
}
