using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.IdentityModel.Tokens;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Core.Authentication.Authorizer;

public class Functions
{
    private readonly string _key;

    public Functions()
    {
        _key = Environment.GetEnvironmentVariable("JWT_SECRET") ??
               throw new NullReferenceException("Secret is missing!");
    }

    public APIGatewayCustomAuthorizerResponse ValidateToken(APIGatewayCustomAuthorizerRequest request,
        ILambdaContext context)
    {
        var response =
            new APIGatewayCustomAuthorizerResponse()
            {
                PolicyDocument = new APIGatewayCustomAuthorizerPolicy()
                {
                    Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                    {
                        new()
                        {
                            Effect = "Allow",
                            Resource = new HashSet<string> { "*" },
                            Action = new HashSet<string> { "execute-api:Invoke" }
                        }
                    }
                }
            };

        var headers = JsonSerializer.Serialize(request,
            new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve });

        var authToken = request.Headers["Authorization"];

        if (authToken == null) return response;

        var claimsPrincipal = GetClaimsPrincipal(authToken);
        if (claimsPrincipal == null) return response;

        var contextOutput = new APIGatewayCustomAuthorizerContextOutput
        {
            [ClaimTypes.NameIdentifier] = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            [ClaimTypes.Name] = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value,
            [ClaimTypes.Email] = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value,
        };
        
        response.Context = contextOutput;

        return response;
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