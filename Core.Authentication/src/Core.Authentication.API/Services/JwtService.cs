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

        if (string.IsNullOrEmpty(_settings.Value.Secret))
        {
            //TODO: Custom Exceptions
            throw new Exception("Missing JWT Secret!");
        }
    }

    public string Generate(CustomerDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret));        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);           
        var tokenDescriptor = new JwtSecurityToken(_settings.Value.Issuer, _settings.Value.Issuer, claims, 
            expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);  

        
    }
}