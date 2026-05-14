namespace RentACar.API.Models;

public class Rental
{
    public int      RentalId       { get; set; }
    public int      CarId          { get; set; }
    public int      UserId         { get; set; }
    public DateTime RentDate       { get; set; }
    public DateTime ReturnDate     { get; set; }
    public decimal  TotalRentPrice { get; set; }
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;

    public Car?  Car  { get; set; }
    public User? User { get; set; }
}
