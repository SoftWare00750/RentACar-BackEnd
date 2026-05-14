using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RentACar.API.Models;

namespace RentACar.API.Services;

public interface ITokenService
{
    (string token, string expiration) CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public (string token, string expiration) CreateToken(User user)
    {
        var key     = _config["Jwt:Key"]    ?? throw new InvalidOperationException("Jwt:Key not configured");
        var issuer  = _config["Jwt:Issuer"] ?? "RentACar";
        var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 480;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var expiry      = DateTime.UtcNow.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            // Standard claims consumed by the Angular JWT library
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name,           $"{user.FirstName} {user.LastName}"),
            new("email",                   user.Email),
            new(ClaimTypes.Role,           user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           issuer,
            claims:             claims,
            expires:            expiry,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry.ToString("o"));
    }
}
