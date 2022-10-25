using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Core.Authentication.Authorizer;

public class Functions
{
    private readonly string _key;

    public Functions()
    {
        _key = Environment.GetEnvironmentVariable("SECRET") ??
               throw new NullReferenceException("JWT Secret is missing!");
    }

    public APIGatewayCustomAuthorizerResponse ValidateToken(APIGatewayCustomAuthorizerRequest request)
    {
        var authToken = request.Headers[HeaderNames.Authorization] ??
                        throw new NullReferenceException("Missing Authorization header");
        
        //TODO: Use HttpClient to call Auth API validate endpoint rather than parsing internally

        var claimsPrincipal = GetClaimsPrincipal(authToken);
        var effect = claimsPrincipal == null ? "Deny" : "Allow";
        var principalId = claimsPrincipal == null ? "401" : claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        return new APIGatewayCustomAuthorizerResponse()
        {
            PrincipalID = principalId,
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy()
            {
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement()
                    {
                        Effect = effect,
                        Resource = new HashSet<string> { "*" },
                        Action = new HashSet<string> { "execute-api:Invoke" }
                    }
                }
            }
        };
    }

    private ClaimsPrincipal? GetClaimsPrincipal(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
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