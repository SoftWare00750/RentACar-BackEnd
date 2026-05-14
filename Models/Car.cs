namespace RentACar.API.Models;

public class Car
{
    public int    CarId       { get; set; }
    public int    BrandId     { get; set; }
    public int    ColorId     { get; set; }
    public string CarName     { get; set; } = string.Empty;
    public string ModelYear   { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public string Description { get; set; } = string.Empty;

    // Navigation
    public Brand? Brand  { get; set; }
    public Color? Color  { get; set; }
    public ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();
    public ICollection<Rental>   Rentals   { get; set; } = new List<Rental>();
}
