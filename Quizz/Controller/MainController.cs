﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Quizz.Controller
{
    public class MainFormController
    {
        private readonly string connectionString;

        public MainFormController()
        {
            connectionString = "Server=localhost;Database=quizz;User ID=root;Password=" + Crypto.Decrypt("kt0xTyBNQsGrjbt16LKj+Q==") + ";SslMode=None;";
        }

        public DataTable LoadThemes()
        {
            string query = "SELECT id, name FROM Thème";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable themesTable = new DataTable();
                    adapter.Fill(themesTable);
                    return themesTable;
                }
            }
        }

        public void AddTheme(string themeName)
        {
            string query = "INSERT INTO Thème (name) VALUES (@Name)";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", themeName);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EditTheme(int id, string newName)
        {
            string query = "UPDATE Thème SET name = @Name WHERE id = @Id";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", newName);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTheme(int id)
        {
            string query = "DELETE FROM Thème WHERE id = @Id";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
