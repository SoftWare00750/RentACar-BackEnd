using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.Core.Utilities.Security.Hashing;
using RentACar.Core.Utilities.Security.JWT;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user, "Registration successful");
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>("User not found");
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Password is incorrect");
            }

            return new SuccessDataResult<User>(userToCheck, "Login successful");
        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult("User already exists");
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            // Convert RentACar.Entities.Concrete.User to RentACar.Core.Entities.Concrete.User
            var coreUser = new Core.Entities.Concrete.User
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                Status = user.Status
            };

            // Convert RentACar.Entities.Concrete.OperationClaim to RentACar.Core.Entities.Concrete.OperationClaim
            var claims = _userService.GetClaims(user);
            var coreClaims = claims.Select(c => new Core.Entities.Concrete.OperationClaim
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            var accessToken = _tokenHelper.CreateToken(coreUser, coreClaims);
            return new SuccessDataResult<AccessToken>(accessToken, "Access token created");
        }
    }
}