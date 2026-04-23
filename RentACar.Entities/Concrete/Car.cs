using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Entities.Concrete
{
    public class Car : IEntity
    {
        public int CarId { get; set; }
        public int BrandId { get; set; }
        public int ColorId { get; set; }
        public string? CarName { get; set; }
        public string ModelYear { get; set; } = string.Empty;
        public decimal DailyPrice { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}