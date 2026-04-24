using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Rental_Business_System.Data;

internal sealed class AuthService
{
    private readonly string _connectionString;

    internal AuthService(string connectionString)
    {
        _connectionString = connectionString;
    }

    internal UserSession? TryLogin(string usernameOrEmail, string password, string requiredRole)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
SELECT Id, Username, Role
FROM Users
WHERE (Username = @value OR Username = LOWER(@value))
  AND PasswordHash = @passwordHash
  AND Role = @role
LIMIT 1;";
        command.Parameters.AddWithValue("@value", usernameOrEmail.Trim());
        command.Parameters.AddWithValue("@passwordHash", HashPassword(password));
        command.Parameters.AddWithValue("@role", requiredRole);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new UserSession
        {
            UserId = reader.GetInt64(0),
            Username = reader.GetString(1),
            Role = reader.GetString(2)
        };
    }

    internal (bool success, string message) RegisterClient(string email, string password)
    {
        string normalized = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized) || !normalized.Contains('@'))
        {
            return (false, "Please enter a valid email.");
        }

        if (password.Length < 4)
        {
            return (false, "Password must be at least 4 characters.");
        }

        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand exists = connection.CreateCommand();
        exists.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @username";
        exists.Parameters.AddWithValue("@username", normalized);
        long count = (long)(exists.ExecuteScalar() ?? 0L);

        if (count > 0)
        {
            return (false, "An account with that email already exists.");
        }

        using SqliteTransaction tx = connection.BeginTransaction();
        using SqliteCommand createUser = connection.CreateCommand();
        createUser.Transaction = tx;
        createUser.CommandText = @"
INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
VALUES (@username, @hash, 'Client', @createdAt);
SELECT last_insert_rowid();";
        createUser.Parameters.AddWithValue("@username", normalized);
        createUser.Parameters.AddWithValue("@hash", HashPassword(password));
        createUser.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        long userId = (long)(createUser.ExecuteScalar() ?? 0L);

        using SqliteCommand createCustomer = connection.CreateCommand();
        createCustomer.Transaction = tx;
        createCustomer.CommandText = @"
INSERT INTO Customers (FullName, Phone, Email, Address, UserId, CreatedAt)
VALUES (@fullName, '', @email, '', @userId, @createdAt);";
        createCustomer.Parameters.AddWithValue("@fullName", normalized.Split('@')[0]);
        createCustomer.Parameters.AddWithValue("@email", normalized);
        createCustomer.Parameters.AddWithValue("@userId", userId);
        createCustomer.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        createCustomer.ExecuteNonQuery();

        tx.Commit();
        return (true, "Registration successful. You can now log in.");
    }

    internal static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
