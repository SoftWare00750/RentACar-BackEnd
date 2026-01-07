using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Entities.Concrete
{
    public class Car : IEntity
    {
        public int CarId { get; set; }

        [Required(ErrorMessage = "Brand ID is required")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Color ID is required")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Model year is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Model year must be 4 characters")]
        public string ModelYear { get; set; }

        [Required(ErrorMessage = "Daily price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Daily price must be greater than 0")]
        public decimal DailyPrice { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [NotMapped]
        public string BrandName { get; set; }

        [NotMapped]
        public string ColorName { get; set; }
    }
}