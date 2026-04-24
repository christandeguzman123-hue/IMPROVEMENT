using MySql.Data.MySqlClient;
using RentalBusinessSystem.Models;
using System;
using System.Collections.Generic;

namespace RentalBusinessSystem.Data
{
    public class RentalRepository
    {
        public bool CreateRental(Rental rental)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO rentals (item_id, customer_name, customer_email, customer_phone, " +
                                 "rental_start_date, rental_end_date, quantity, total_cost, payment_method, " +
                                 "payment_status, rental_status, created_date) " +
                                 "VALUES (@item_id, @customer_name, @customer_email, @customer_phone, " +
                                 "@rental_start_date, @rental_end_date, @quantity, @total_cost, @payment_method, " +
                                 "@payment_status, @rental_status, @created_date)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@item_id", rental.ItemId);
                    cmd.Parameters.AddWithValue("@customer_name", rental.CustomerName);
                    cmd.Parameters.AddWithValue("@customer_email", rental.CustomerEmail);
                    cmd.Parameters.AddWithValue("@customer_phone", rental.CustomerPhone);
                    cmd.Parameters.AddWithValue("@rental_start_date", rental.RentalStartDate);
                    cmd.Parameters.AddWithValue("@rental_end_date", rental.RentalEndDate);
                    cmd.Parameters.AddWithValue("@quantity", rental.Quantity);
                    cmd.Parameters.AddWithValue("@total_cost", rental.TotalCost);
                    cmd.Parameters.AddWithValue("@payment_method", rental.PaymentMethod);
                    cmd.Parameters.AddWithValue("@payment_status", rental.PaymentStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@rental_status", rental.RentalStatus ?? "Active");
                    cmd.Parameters.AddWithValue("@created_date", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating rental: " + ex.Message);
            }
        }

        public List<Rental> GetAllRentals()
        {
            var rentals = new List<Rental>();
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM rentals ORDER BY created_date DESC";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rentals.Add(new Rental
                            {
                                Id = (int)reader["id"],
                                ItemId = (int)reader["item_id"],
                                CustomerName = reader["customer_name"].ToString(),
                                CustomerEmail = reader["customer_email"].ToString(),
                                CustomerPhone = reader["customer_phone"].ToString(),
                                RentalStartDate = (DateTime)reader["rental_start_date"],
                                RentalEndDate = (DateTime)reader["rental_end_date"],
                                Quantity = (int)reader["quantity"],
                                TotalCost = (decimal)reader["total_cost"],
                                PaymentMethod = reader["payment_method"].ToString(),
                                PaymentStatus = reader["payment_status"].ToString(),
                                RentalStatus = reader["rental_status"].ToString(),
                                CreatedDate = (DateTime)reader["created_date"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving rentals: " + ex.Message);
            }
            return rentals;
        }

        public Rental GetRentalById(int rentalId)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM rentals WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", rentalId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rental
                            {
                                Id = (int)reader["id"],
                                ItemId = (int)reader["item_id"],
                                CustomerName = reader["customer_name"].ToString(),
                                CustomerEmail = reader["customer_email"].ToString(),
                                CustomerPhone = reader["customer_phone"].ToString(),
                                RentalStartDate = (DateTime)reader["rental_start_date"],
                                RentalEndDate = (DateTime)reader["rental_end_date"],
                                Quantity = (int)reader["quantity"],
                                TotalCost = (decimal)reader["total_cost"],
                                PaymentMethod = reader["payment_method"].ToString(),
                                PaymentStatus = reader["payment_status"].ToString(),
                                RentalStatus = reader["rental_status"].ToString(),
                                CreatedDate = (DateTime)reader["created_date"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving rental: " + ex.Message);
            }
            return null;
        }

        public bool UpdateRentalStatus(int rentalId, string status)
        {
            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE rentals SET rental_status = @status WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", rentalId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating rental status: " + ex.Message);
            }
        }
    }
}
