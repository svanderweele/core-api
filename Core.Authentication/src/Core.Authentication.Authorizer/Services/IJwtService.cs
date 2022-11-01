using System.Security.Claims;

namespace Core.Authentication.Authorizer.Services;

public interface IJwtService
{
    ClaimsPrincipal? ValidateToken(string authToken);
}