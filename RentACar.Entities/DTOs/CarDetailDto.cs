using RentACar.Core.Entities;

namespace RentACar.Entities.DTOs
{
    /// <summary>
    /// Matches the Angular Car interface:
    /// carId, colorId, brandId, carName, colorName, brandName,
    /// modelYear, dailyPrice, description, imagePath
    /// </summary>
    public class CarDetailDto : IEntity
    {
        public int CarId { get; set; }
        public int BrandId { get; set; }
        public int ColorId { get; set; }

        // CarName - uses the Car.CarName if set, otherwise brandName + modelYear
        public string CarName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string ModelYear { get; set; } = string.Empty;
        public decimal DailyPrice { get; set; }
        public string Description { get; set; } = string.Empty;

        // ImagePath: default image if no car image uploaded
        public string ImagePath { get; set; } = "/images/default-car.jpg";
    }
}