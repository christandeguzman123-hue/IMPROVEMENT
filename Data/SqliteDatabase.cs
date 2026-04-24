using Microsoft.Data.Sqlite;

namespace Rental_Business_System.Data;

internal static class SqliteDatabase
{
    private static readonly string DbPath = Path.Combine(AppContext.BaseDirectory, "rental_system.db");

    internal static string ConnectionString => $"Data Source={DbPath};";

    internal static void Initialize()
    {
        using SqliteConnection connection = new(ConnectionString);
        connection.Open();

        string schema = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Customers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Phone TEXT,
    Email TEXT,
    Address TEXT,
    UserId INTEGER,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS Inventory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ItemName TEXT NOT NULL,
    Category TEXT,
    DailyRate REAL NOT NULL,
    QuantityAvailable INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Rentals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    CustomerId INTEGER,
    ItemId INTEGER NOT NULL,
    Item TEXT NOT NULL,
    Username TEXT,
    Quantity INTEGER NOT NULL,
    StartDate TEXT NOT NULL,
    EndDate TEXT,
    Status TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id),
    FOREIGN KEY(CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY(ItemId) REFERENCES Inventory(Id)
);
";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = schema;
            command.ExecuteNonQuery();
        }

        using (SqliteCommand normalizeStatuses = connection.CreateCommand())
        {
            normalizeStatuses.CommandText = "UPDATE Rentals SET Status = 'Approved' WHERE Status = 'Active';";
            normalizeStatuses.ExecuteNonQuery();
        }

        SeedDefaults(connection);
    }

    private static void SeedDefaults(SqliteConnection connection)
    {
        using SqliteCommand userCount = connection.CreateCommand();
        userCount.CommandText = "SELECT COUNT(1) FROM Users";
        long users = (long)(userCount.ExecuteScalar() ?? 0L);

        if (users == 0)
        {
            using SqliteCommand addAdmin = connection.CreateCommand();
            addAdmin.CommandText = "INSERT INTO Users (Username, PasswordHash, Role, CreatedAt) VALUES (@user, @hash, @role, @createdAt)";
            addAdmin.Parameters.AddWithValue("@user", "admin");
            addAdmin.Parameters.AddWithValue("@hash", AuthService.HashPassword("admin123"));
            addAdmin.Parameters.AddWithValue("@role", "Admin");
            addAdmin.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
            addAdmin.ExecuteNonQuery();
        }

        using SqliteCommand itemCount = connection.CreateCommand();
        itemCount.CommandText = "SELECT COUNT(1) FROM Inventory";
        long items = (long)(itemCount.ExecuteScalar() ?? 0L);

        if (items == 0)
        {
            string[] seedCommands =
            {
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Camera', 'Video', 850, 5, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Projector', 'Display', 1100, 3, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Speaker', 'Audio', 550, 4, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Laptop', 'Computing', 1400, 2, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Microphone', 'Audio', 450, 6, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Lighting Kit', 'Studio', 1000, 4, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Tripod', 'Studio', 450, 7, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Drone', 'Video', 1700, 2, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('Tablet', 'Mobile', 400, 3, @ts)",
                "INSERT INTO Inventory (ItemName, Category, DailyRate, QuantityAvailable, CreatedAt) VALUES ('VR Headset', 'Gaming', 1700, 1, @ts)"
            };

            foreach (string sql in seedCommands)
            {
                using SqliteCommand insert = connection.CreateCommand();
                insert.CommandText = sql;
                insert.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("O"));
                insert.ExecuteNonQuery();
            }
        }
    }
}
