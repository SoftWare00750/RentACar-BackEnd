namespace RentACar.API.Models;

public class CarImage
{
    public int      ImageId   { get; set; }
    public int      CarId     { get; set; }
    public string   ImagePath { get; set; } = "/uploads/default.jpg";
    public DateTime Date      { get; set; } = DateTime.UtcNow;

    public Car? Car { get; set; }
}
