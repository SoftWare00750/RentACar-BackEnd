using RentACar.Core.Entities;

namespace RentACar.Entities.DTOs
{
    public class UserForLoginDto : IEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}