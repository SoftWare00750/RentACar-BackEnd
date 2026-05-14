namespace RentACar.API.Models;

public class Brand
{
    public int    BrandId   { get; set; }
    public string BrandName { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}
