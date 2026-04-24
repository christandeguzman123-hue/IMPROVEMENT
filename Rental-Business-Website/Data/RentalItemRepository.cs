using MySql.Data.MySqlClient;
using RentalBusinessSystem.Models;
using System;
using System.Collections.Generic;

namespace RentalBusinessSystem.Data
{
    public class RentalItemRepository
    {
        public List<RentalItem> GetAllItems()
        {
            var items = new List<RentalItem>();
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM rental_items WHERE is_active = 1";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new RentalItem
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                DailyRate = (decimal)reader["daily_rate"],
                                QuantityAvailable = (int)reader["quantity_available"],
                                Category = reader["category"].ToString(),
                                ImagePath = reader["image_path"].ToString() ?? "",
                                CreatedDate = (DateTime)reader["created_date"],
                                IsActive = (bool)reader["is_active"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving rental items: " + ex.Message);
            }
            return items;
        }

        public RentalItem GetItemById(int itemId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM rental_items WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", itemId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RentalItem
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                DailyRate = (decimal)reader["daily_rate"],
                                QuantityAvailable = (int)reader["quantity_available"],
                                Category = reader["category"].ToString(),
                                ImagePath = reader["image_path"].ToString() ?? "",
                                CreatedDate = (DateTime)reader["created_date"],
                                IsActive = (bool)reader["is_active"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving rental item: " + ex.Message);
            }
            return null;
        }

        public bool AddItem(RentalItem item)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO rental_items (name, description, daily_rate, quantity_available, category, image_path, created_date, is_active) " +
                                 "VALUES (@name, @description, @daily_rate, @quantity, @category, @image_path, @created_date, 1)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@description", item.Description ?? "");
                    cmd.Parameters.AddWithValue("@daily_rate", item.DailyRate);
                    cmd.Parameters.AddWithValue("@quantity", item.QuantityAvailable);
                    cmd.Parameters.AddWithValue("@category", item.Category);
                    cmd.Parameters.AddWithValue("@image_path", item.ImagePath ?? "");
                    cmd.Parameters.AddWithValue("@created_date", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding rental item: " + ex.Message);
            }
        }

        public bool UpdateItem(RentalItem item)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE rental_items SET name=@name, description=@description, daily_rate=@daily_rate, " +
                                 "quantity_available=@quantity, category=@category, image_path=@image_path WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@description", item.Description ?? "");
                    cmd.Parameters.AddWithValue("@daily_rate", item.DailyRate);
                    cmd.Parameters.AddWithValue("@quantity", item.QuantityAvailable);
                    cmd.Parameters.AddWithValue("@category", item.Category);
                    cmd.Parameters.AddWithValue("@image_path", item.ImagePath ?? "");
                    cmd.Parameters.AddWithValue("@id", item.Id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating rental item: " + ex.Message);
            }
        }

        public bool DeleteItem(int itemId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE rental_items SET is_active = 0 WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", itemId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting rental item: " + ex.Message);
            }
        }
    }
}
