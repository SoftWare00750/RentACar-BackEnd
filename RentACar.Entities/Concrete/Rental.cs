using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Entities.Concrete
{
    public class Rental : IEntity
    {
        public int RentalId { get; set; }

        [Required(ErrorMessage = "Car ID is required")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Rent date is required")]
        public DateTime RentDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}