using RentACar.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Entities.DTOs
{
    public class UserForLoginDto : IEntity
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}