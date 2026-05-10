using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.Core.Utilities.Security.Hashing;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user));
        }

        public void Add(User user)
        {
            _userDal.Add(user);
        }

        public User? GetByMail(string email)
        {
            return _userDal.Get(u => u.Email == email);
        }

        public IResult Update(User user)
        {
            _userDal.Update(user);
            return new SuccessResult("User updated.");
        }

        public IResult Delete(User user)
        {
            _userDal.Delete(user);
            return new SuccessResult("User deleted.");
        }

        public IDataResult<User> GetById(int userId)
        {
            var user = _userDal.Get(u => u.UserId == userId);
            if (user == null)
                return new ErrorDataResult<User>("User not found");
            return new SuccessDataResult<User>(user);
        }

        public IResult ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userDal.Get(u => u.UserId == userId);
            if (user == null)
                return new ErrorResult("User not found");

            if (!HashingHelper.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                return new ErrorResult("Current password is incorrect");

            byte[] newHash, newSalt;
            HashingHelper.CreatePasswordHash(newPassword, out newHash, out newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;
            _userDal.Update(user);
            return new SuccessResult("Password changed successfully");
        }
    }
}