namespace Rental_Business_System.Data;

internal sealed class UserSession
{
    public long UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
