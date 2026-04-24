using MySql.Data.MySqlClient;
using System;

namespace RentalBusinessSystem.Data
{
    public class DatabaseConnection
    {
        private static readonly string ConnectionString = "Server=localhost;Database=rental_business_db;Uid=root;Pwd=;Port=3306;";

        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create database connection: " + ex.Message);
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
