using System.Data;
using Microsoft.Data.Sqlite;

namespace Rental_Business_System.Data;

internal sealed class RentalRepository
{
    private readonly string _connectionString;
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Approved",
        "Active",
        "Returned",
        "Cancelled"
    };

    internal RentalRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    internal DataTable GetCustomers()
    {
        const string sql = @"
SELECT Id, FullName, Phone, Email, Address
FROM Customers
ORDER BY FullName;";
        return Query(sql);
    }

    internal void AddCustomer(string fullName, string phone, string email, string address)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Customers (FullName, Phone, Email, Address, CreatedAt)
VALUES (@name, @phone, @email, @address, @createdAt);";
        command.Parameters.AddWithValue("@name", fullName);
        command.Parameters.AddWithValue("@phone", phone);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@address", address);
        command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    internal void UpdateCustomer(long id, string fullName, string phone, string email, string address)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
UPDATE Customers
SET FullName = @name, Phone = @phone, Email = @email, Address = @address
WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", fullName);
        command.Parameters.AddWithValue("@phone", phone);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@address", address);
        command.ExecuteNonQuery();
    }

    internal void DeleteCustomer(long id)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Customers WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    internal DataTable GetInventory()
    {
        const string sql = @"
SELECT Id, ItemName, Category, DailyRate, QuantityAvailable
FROM Inventory
ORDER BY ItemName;";
        return Query(sql);
    }

    internal DataTable GetAvailableInventory()
    {
        const string sql = @"
SELECT Id, ItemName, Category, DailyRate, QuantityAvailable
FROM Inventory
WHERE QuantityAvailable > 0
ORDER BY ItemName;";
        return Query(sql);
    }

    internal void AddInventoryItem(string itemName, string category, decimal dailyRate, int quantityAvailable)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt)
VALUES (@name, @category, @dailyRate, @qty, @createdAt);";
        command.Parameters.AddWithValue("@name", itemName);
        command.Parameters.AddWithValue("@category", category);
        command.Parameters.AddWithValue("@dailyRate", dailyRate);
        command.Parameters.AddWithValue("@qty", quantityAvailable);
        command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    internal void UpdateInventoryItem(long id, string itemName, string category, decimal dailyRate, int quantityAvailable)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
UPDATE Inventory
SET ItemName = @name, Category = @category, DailyRate = @dailyRate, QuantityAvailable = @qty
WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", itemName);
        command.Parameters.AddWithValue("@category", category);
        command.Parameters.AddWithValue("@dailyRate", dailyRate);
        command.Parameters.AddWithValue("@qty", quantityAvailable);
        command.ExecuteNonQuery();
    }

    internal void DeleteInventoryItem(long id)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Inventory WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    internal DataTable GetRentals()
    {
        const string sql = @"
SELECT r.Id,
       COALESCE(c.FullName, r.Username, 'Guest') AS Customer,
       r.Item,
       r.Quantity,
       r.StartDate,
       r.EndDate,
       r.Status
FROM Rentals r
LEFT JOIN Customers c ON c.Id = r.CustomerId
ORDER BY r.CreatedAt DESC;";
        return Query(sql);
    }

    internal DataTable GetRentalsByUser(long userId)
    {
        const string sql = @"
SELECT r.Id,
       r.Item,
       r.Quantity,
       r.StartDate,
       r.EndDate,
       r.Status
FROM Rentals r
WHERE r.UserId = @userId
ORDER BY r.CreatedAt DESC;";
        return Query(sql, ("@userId", userId));
    }

    internal void CreateRental(long customerId, long itemId, int quantity, DateTime startDate, DateTime? endDate)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteTransaction tx = connection.BeginTransaction();

        (string itemName, int available) = GetInventorySnapshot(connection, tx, itemId);
        if (available < quantity)
        {
            throw new InvalidOperationException("Not enough inventory available for this item.");
        }

        using SqliteCommand insert = connection.CreateCommand();
        insert.Transaction = tx;
        insert.CommandText = @"
INSERT INTO Rentals (CustomerId, ItemId, Item, Quantity, StartDate, EndDate, Status, CreatedAt)
VALUES (@customerId, @itemId, @item, @quantity, @startDate, @endDate, @status, @createdAt);";
        insert.Parameters.AddWithValue("@customerId", customerId);
        insert.Parameters.AddWithValue("@itemId", itemId);
        insert.Parameters.AddWithValue("@item", itemName);
        insert.Parameters.AddWithValue("@quantity", quantity);
        insert.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
        insert.Parameters.AddWithValue("@endDate", endDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        insert.Parameters.AddWithValue("@status", "Approved");
        insert.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        insert.ExecuteNonQuery();

        UpdateInventoryQuantity(connection, tx, itemId, available - quantity);
        tx.Commit();
    }

    internal void CreateRentalForUser(long userId, string username, long itemId, int quantity, DateTime startDate, DateTime? endDate)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteTransaction tx = connection.BeginTransaction();

        (string itemName, int available) = GetInventorySnapshot(connection, tx, itemId);
        if (available < quantity)
        {
            throw new InvalidOperationException("Item is currently out of stock.");
        }

        long? customerId = GetCustomerIdByUser(connection, tx, userId);

        using SqliteCommand insert = connection.CreateCommand();
        insert.Transaction = tx;
        insert.CommandText = @"
INSERT INTO Rentals (UserId, CustomerId, ItemId, Item, Username, Quantity, StartDate, EndDate, Status, CreatedAt)
VALUES (@userId, @customerId, @itemId, @item, @username, @quantity, @startDate, @endDate, @status, @createdAt);";
        insert.Parameters.AddWithValue("@userId", userId);
        insert.Parameters.AddWithValue("@customerId", customerId ?? (object)DBNull.Value);
        insert.Parameters.AddWithValue("@itemId", itemId);
        insert.Parameters.AddWithValue("@item", itemName);
        insert.Parameters.AddWithValue("@username", username);
        insert.Parameters.AddWithValue("@quantity", quantity);
        insert.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
        insert.Parameters.AddWithValue("@endDate", endDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        insert.Parameters.AddWithValue("@status", "Pending");
        insert.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        insert.ExecuteNonQuery();

        UpdateInventoryQuantity(connection, tx, itemId, available - quantity);
        tx.Commit();
    }

    internal void UpdateRentalStatus(long rentalId, string status)
    {
        if (!AllowedStatuses.Contains(status))
        {
            throw new InvalidOperationException($"Unsupported rental status '{status}'.");
        }

        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE Rentals SET Status = @status WHERE Id = @id;";
        command.Parameters.AddWithValue("@status", status);
        command.Parameters.AddWithValue("@id", rentalId);

        if (command.ExecuteNonQuery() == 0)
        {
            throw new InvalidOperationException("Rental not found.");
        }
    }

    internal void MarkRentalReturned(long rentalId)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteTransaction tx = connection.BeginTransaction();

        using SqliteCommand select = connection.CreateCommand();
        select.Transaction = tx;
        select.CommandText = "SELECT ItemId, Quantity, Status FROM Rentals WHERE Id = @id LIMIT 1;";
        select.Parameters.AddWithValue("@id", rentalId);

        using SqliteDataReader reader = select.ExecuteReader();
        if (!reader.Read())
        {
            throw new InvalidOperationException("Rental not found.");
        }

        long itemId = reader.GetInt64(0);
        int quantity = reader.GetInt32(1);
        string status = reader.GetString(2);
        reader.Close();

        if (string.Equals(status, "Returned", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using SqliteCommand update = connection.CreateCommand();
        update.Transaction = tx;
        update.CommandText = "UPDATE Rentals SET Status = 'Returned' WHERE Id = @id;";
        update.Parameters.AddWithValue("@id", rentalId);
        update.ExecuteNonQuery();

        (string _, int available) = GetInventorySnapshot(connection, tx, itemId);
        UpdateInventoryQuantity(connection, tx, itemId, available + quantity);

        tx.Commit();
    }

    internal (int totalCustomers, int activeRentals, int overdueRentals) GetDashboardCounts(long? userId)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        int totalCustomers;
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(1) FROM Customers;";
            totalCustomers = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        int activeRentals;
        int overdueRentals;

        if (userId.HasValue)
        {
            using SqliteCommand active = connection.CreateCommand();
            active.CommandText = "SELECT COUNT(1) FROM Rentals WHERE UserId = @userId AND Status IN ('Approved', 'Active');";
            active.Parameters.AddWithValue("@userId", userId.Value);
            activeRentals = Convert.ToInt32(active.ExecuteScalar() ?? 0);

            using SqliteCommand overdue = connection.CreateCommand();
            overdue.CommandText = @"
SELECT COUNT(1)
FROM Rentals
WHERE UserId = @userId
    AND Status IN ('Approved', 'Active')
  AND EndDate IS NOT NULL
  AND date(EndDate) < date('now');";
            overdue.Parameters.AddWithValue("@userId", userId.Value);
            overdueRentals = Convert.ToInt32(overdue.ExecuteScalar() ?? 0);
        }
        else
        {
            using SqliteCommand active = connection.CreateCommand();
                        active.CommandText = "SELECT COUNT(1) FROM Rentals WHERE Status IN ('Approved', 'Active');";
            activeRentals = Convert.ToInt32(active.ExecuteScalar() ?? 0);

            using SqliteCommand overdue = connection.CreateCommand();
            overdue.CommandText = @"
SELECT COUNT(1)
FROM Rentals
WHERE Status IN ('Approved', 'Active')
  AND EndDate IS NOT NULL
  AND date(EndDate) < date('now');";
            overdueRentals = Convert.ToInt32(overdue.ExecuteScalar() ?? 0);
        }

        return (totalCustomers, activeRentals, overdueRentals);
    }

    internal DataTable GetSummary()
    {
        DataTable table = new();
        table.Columns.Add("Metric", typeof(string));
        table.Columns.Add("Value", typeof(string));

        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        table.Rows.Add("Total Customers", ScalarInt(connection, "SELECT COUNT(1) FROM Customers;"));
        table.Rows.Add("Inventory Items", ScalarInt(connection, "SELECT COUNT(1) FROM Inventory;"));
        table.Rows.Add("Pending Rentals", ScalarInt(connection, "SELECT COUNT(1) FROM Rentals WHERE Status = 'Pending';"));
        table.Rows.Add("Approved Rentals", ScalarInt(connection, "SELECT COUNT(1) FROM Rentals WHERE Status IN ('Approved', 'Active');"));
        table.Rows.Add("Returned Rentals", ScalarInt(connection, "SELECT COUNT(1) FROM Rentals WHERE Status = 'Returned';"));

        return table;
    }

    private static int ScalarInt(SqliteConnection connection, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt32(command.ExecuteScalar() ?? 0);
    }

    private static long? GetCustomerIdByUser(SqliteConnection connection, SqliteTransaction tx, long userId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = "SELECT Id FROM Customers WHERE UserId = @userId LIMIT 1;";
        command.Parameters.AddWithValue("@userId", userId);
        object? value = command.ExecuteScalar();
        return value is null || value is DBNull ? null : Convert.ToInt64(value);
    }

    private static (string itemName, int available) GetInventorySnapshot(SqliteConnection connection, SqliteTransaction tx, long itemId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = "SELECT ItemName, QuantityAvailable FROM Inventory WHERE Id = @id LIMIT 1;";
        command.Parameters.AddWithValue("@id", itemId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new InvalidOperationException("Inventory item not found.");
        }

        string itemName = reader.GetString(0);
        int available = reader.GetInt32(1);
        return (itemName, available);
    }

    private static void UpdateInventoryQuantity(SqliteConnection connection, SqliteTransaction tx, long itemId, int newQuantity)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = "UPDATE Inventory SET QuantityAvailable = @qty WHERE Id = @id;";
        command.Parameters.AddWithValue("@qty", Math.Max(0, newQuantity));
        command.Parameters.AddWithValue("@id", itemId);
        command.ExecuteNonQuery();
    }

    private DataTable Query(string sql, params (string name, object value)[] parameters)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        foreach ((string name, object value) in parameters)
        {
            command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        using SqliteDataReader reader = command.ExecuteReader();
        DataTable table = new();
        table.Load(reader);
        return table;
    }
}
