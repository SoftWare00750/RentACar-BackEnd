namespace RentACar.API.Models;

public class Color
{
    public int    ColorId   { get; set; }
    public string ColorName { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}
