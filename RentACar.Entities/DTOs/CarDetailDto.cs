using RentACar.Core.Entities;

namespace RentACar.Entities.DTOs
{
    public class CarDetailDto : IEntity
    {
        public int CarId { get; set; }
        public string BrandName { get; set; }
        public string ColorName { get; set; }
        public string ModelYear { get; set; }
        public decimal DailyPrice { get; set; }
        public string Description { get; set; }
    }
}