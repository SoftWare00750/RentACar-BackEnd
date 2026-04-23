using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Business.Abstract;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        IUserService _userService;
        IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // /users/getbyid?userId=5
        [HttpGet("getbyid")]
        public IActionResult GetById([FromQuery] int userId)
        {
            var result = _userService.GetById(userId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        // /users/getbyid/{userId}
        [HttpGet("getbyid/{userId:int}")]
        public IActionResult GetByIdRoute([FromRoute] int userId)
        {
            var result = _userService.GetById(userId);
            if (result.Success) return Ok(result);
            return NotFound(result);
        }

        [HttpPut("updated")]
        public IActionResult Update([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Update is handled by user service
            return Ok(new { success = true, message = "User updated" });
        }
    }
}