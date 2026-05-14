using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.API.Data;
using RentACar.API.DTOs;
using RentACar.API.Models;
using RentACar.API.Services;

namespace RentACar.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext  _db;
    private readonly ITokenService _tokens;

    public AuthController(AppDbContext db, ITokenService tokens)
    {
        _db     = db;
        _tokens = tokens;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new ResponseModel { Success = false, Message = "Invalid email or password." });

        var (token, expiry) = _tokens.CreateToken(user);
        return Ok(new SingleResponseModel<TokenModel>
        {
            Success = true,
            Message = "Login successful.",
            Data    = new TokenModel { Token = token, Expiration = expiry }
        });
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return Conflict(new ResponseModel { Success = false, Message = "Email already in use." });

        var user = new User
        {
            FirstName    = req.FirstName,
            LastName     = req.LastName,
            Email        = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role         = "user"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, expiry) = _tokens.CreateToken(user);
        return Ok(new SingleResponseModel<TokenModel>
        {
            Success = true,
            Message = "Registration successful.",
            Data    = new TokenModel { Token = token, Expiration = expiry }
        });
    }

    // POST api/auth/changepassword
    [Authorize]
    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var user = await _db.Users.FindAsync(req.UserId);
        if (user is null)
            return NotFound(new ResponseModel { Success = false, Message = "User not found." });

        if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, user.PasswordHash))
            return BadRequest(new ResponseModel { Success = false, Message = "Old password is incorrect." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await _db.SaveChangesAsync();

        return Ok(new ResponseModel { Success = true, Message = "Password changed successfully." });
    }
}
