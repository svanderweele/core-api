using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Authentication.API.Contracts.Data;
using Core.Authentication.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Authentication.API.Services;

public class JwtService : IJwtService
{
    private readonly IOptions<JwtSettings> _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings;
    }
    
    public string Generate(CustomerDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email), new(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var secret = Encoding.UTF8.GetBytes(_settings.Value.Secret);
        var signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    public ClaimsPrincipal? ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret)),
        };
        try
        {
            return tokenHandler.ValidateToken(authToken, validationParams, out _);
        }
        catch (Exception)
        {
            return null;
        }
    }
}