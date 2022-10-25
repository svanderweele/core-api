using System.Security.Claims;

namespace Core.Gaming.API.Services;

public interface IJwtService
{
    ClaimsPrincipal? ValidateToken(string authToken);
}