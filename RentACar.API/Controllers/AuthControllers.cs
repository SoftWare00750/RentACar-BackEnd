using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Security.Hashing;
using RentACar.Entities.DTOs;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserForLoginDto userForLoginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success) return BadRequest(new { message = userToLogin.Message });

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success) return Ok(result);

            return BadRequest(new { message = result.Message });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success) return BadRequest(new { message = userExists.Message });

            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            if (!registerResult.Success) return BadRequest(new { message = registerResult.Message });

            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success) return Ok(result);

            return BadRequest(new { message = result.Message });
        }

        [HttpPost("changepassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] PasswordChangeDto passwordChangeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userResult = _userService.GetById(passwordChangeDto.UserId);
            if (!userResult.Success) return NotFound(new { message = "User not found" });

            var user = userResult.Data;

            // Verify current password
            if (!HashingHelper.VerifyPasswordHash(passwordChangeDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                return BadRequest(new { message = "Current password is incorrect" });

            // Update password hash
            byte[] newHash, newSalt;
            HashingHelper.CreatePasswordHash(passwordChangeDto.NewPassword, out newHash, out newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            _userService.Update(user);

            return Ok(new { success = true, message = "Password changed successfully" });
        }
    }

    public class PasswordChangeDto
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}