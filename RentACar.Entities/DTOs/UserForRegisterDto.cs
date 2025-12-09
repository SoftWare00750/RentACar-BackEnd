using RentACar.Core.Entities;

namespace RentACar.Entities.DTOs
{
    public class UserForRegisterDto : IEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}