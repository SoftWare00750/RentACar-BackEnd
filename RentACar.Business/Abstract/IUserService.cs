using RentACar.Core.Utilities.Results;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<OperationClaim>> GetClaims(User user);
        void Add(User user);
        User? GetByMail(string email);
        IResult Update(User user);
        IResult Delete(User user);
        IDataResult<User> GetById(int userId);
        IResult ChangePassword(int userId, string oldPassword, string newPassword);
    }
}