using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Entities.Concrete
{
    public class Brand : IEntity
    {
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(50, ErrorMessage = "Brand name cannot be longer than 50 characters")]
        public string BrandName { get; set; }
    }
}