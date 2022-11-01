using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Authentication.Authorizer.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(JwtSettings settings)
    {
        _settings = settings;

        if (string.IsNullOrEmpty(_settings.Secret))
        {
            //TODO: Custom Exceptions
            throw new Exception("Missing JWT Secret!");
        }
    }

    public ClaimsPrincipal? ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
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