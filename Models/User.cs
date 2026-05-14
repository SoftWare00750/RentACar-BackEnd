namespace RentACar.API.Models;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role      { get; set; } = "user";         // "user" | "admin"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
