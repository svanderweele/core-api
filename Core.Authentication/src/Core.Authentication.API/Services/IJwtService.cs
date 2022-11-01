using System.Security.Claims;
using Core.Authentication.API.Contracts.Data;

namespace Core.Authentication.API.Services;

public interface IJwtService
{
    string Generate(CustomerDto customer);
}