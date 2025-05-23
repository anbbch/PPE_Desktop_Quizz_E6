using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace Quizz.Model
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
        public void AddUser(string name, string status, string username, string password, string email)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            using (var cmd = new MySqlCommand("sp_AddUser", connection))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_name", name);
                cmd.Parameters.AddWithValue("@p_status", status);
                cmd.Parameters.AddWithValue("@p_username", username);
                cmd.Parameters.AddWithValue("@p_password", hashedPassword);
                cmd.Parameters.AddWithValue("@p_salt", salt);
                cmd.Parameters.AddWithValue("@p_email", email);
                cmd.ExecuteNonQuery();
            }
        }



        public bool VerifyLogin(string username, string password)
        {
            using (var cmd = new MySqlCommand("sp_GetUserCredentials", connection))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader.GetString("password");
                        string salt = reader.GetString("salt");

                        string hashedPassword = HashPassword(password, salt);

                        return hashedPassword == storedPassword;
                    }
                }
            }
            return false;
        }




        // Méthode pour vérifier le login d'un utilisateur
        //public bool VerifyLogin(string username, string password)
        //{
        //    string query = "SELECT password, salt FROM Users WHERE username = @Username";
        //    using (var cmd = new MySqlCommand(query, connection))
        //    {
        //        cmd.Parameters.AddWithValue("@Username", username);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                string storedPassword = reader.GetString("password");
        //                string salt = reader.GetString("salt");

        //                // Hasher le mot de passe fourni avec le salt stocké
        //                string hashedPassword = HashPassword(password, salt);

        //                // Vérifier si les deux mots de passe correspondent
        //                return hashedPassword == storedPassword;
        //            }
        //        }
        //    }
        //    return false; // Aucun utilisateur trouvé ou mot de passe incorrect
        //}
    }
}
