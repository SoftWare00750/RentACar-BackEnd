using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound(new ResponseModel { Success = false, Message = "User not found." });

        return Ok(new SingleResponseModel<UserDto>
        {
            Success = true, Message = "OK.",
            Data    = new UserDto
            {
                UserId    = user.UserId,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email
            }
        });
    }

    [HttpPut("updated")]
    public async Task<IActionResult> Update([FromBody] UserDto dto)
    {
        var user = await _db.Users.FindAsync(dto.UserId);
        if (user is null) return NotFound(new ResponseModel { Success = false, Message = "User not found." });

        user.FirstName = dto.FirstName;
        user.LastName  = dto.LastName;
        user.Email     = dto.Email;
        await _db.SaveChangesAsync();

        return Ok(new ResponseModel { Success = true, Message = "Profile updated." });
    }
}
