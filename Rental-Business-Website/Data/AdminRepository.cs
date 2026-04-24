using MySql.Data.MySqlClient;
using RentalBusinessSystem.Models;
using System;
using System.Collections.Generic;

namespace RentalBusinessSystem.Data
{
    public class AdminRepository
    {
        public bool AuthenticateAdmin(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM admins WHERE username = @username AND password_hash = MD5(@password)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int result = (int)cmd.ExecuteScalar();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Authentication error: " + ex.Message);
            }
        }

        public Admin GetAdminByUsername(string username)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, email_address, created_date FROM admins WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admin
                            {
                                Id = (int)reader["id"],
                                Username = reader["username"].ToString(),
                                EmailAddress = reader["email_address"].ToString(),
                                CreatedDate = (DateTime)reader["created_date"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving admin: " + ex.Message);
            }
            return null;
        }

        public bool AddAdmin(Admin admin)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO admins (username, password_hash, email_address, created_date, is_active) " +
                                 "VALUES (@username, MD5(@password), @email, @created_date, 1)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", admin.Username);
                    cmd.Parameters.AddWithValue("@password", admin.PasswordHash);
                    cmd.Parameters.AddWithValue("@email", admin.EmailAddress);
                    cmd.Parameters.AddWithValue("@created_date", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding admin: " + ex.Message);
            }
        }
    }
}
