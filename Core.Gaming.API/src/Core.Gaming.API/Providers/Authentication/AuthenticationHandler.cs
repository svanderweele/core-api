using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Core.Gaming.API.Providers.Authentication
{
    public class CoreAuthSchemeOptions
        : AuthenticationSchemeOptions
    {
    }

    public class CoreAuthHandler
        : AuthenticationHandler<CoreAuthSchemeOptions>
    {
        public static readonly string SchemeName = "CoreAuthentication";

        public CoreAuthHandler(
            IOptionsMonitor<CoreAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return AuthenticateResult.Fail("Authorization header not found.");
            }

            var token = Request.Headers[HeaderNames.Authorization].ToString();

            HttpClient client = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://z7qv6ih936.execute-api.eu-west-1.amazonaws.com/prod/api/authentication/validate"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), token },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                }
            };
            
            var response = await client.SendAsync(httpRequestMessage);

            var body = await response.Content.ReadFromJsonAsync<ValidateTokenResponse>();

            var claims = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, body.Id.ToString()),
                    new Claim(ClaimTypes.Name, body.Name),
                    new Claim(ClaimTypes.Email, body.Email),
                })
            });

            //TODO: Call Auth API
            // var claimsPrincipal = _jwtService.ValidateToken(token);
            //
            // if (claimsPrincipal == null)
            // {
            //     return Task.FromResult(AuthenticateResult.Fail("Token Invalid"));
            //
            // }
            return AuthenticateResult.Success(
                new AuthenticationTicket(claims, Scheme.Name));

            return null;
        }
    }
}