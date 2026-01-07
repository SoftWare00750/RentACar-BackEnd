using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Entities.Concrete
{
    public class Color : IEntity
    {
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Color name is required")]
        [StringLength(30, ErrorMessage = "Color name cannot be longer than 30 characters")]
        public string ColorName { get; set; }
    }
}