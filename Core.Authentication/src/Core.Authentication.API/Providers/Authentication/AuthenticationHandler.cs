using System.Text.Encodings.Web;
using Core.Authentication.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Core.Authentication.API.Providers.Authentication
{
    public class CoreAuthSchemeOptions
        : AuthenticationSchemeOptions
    {
    }

    public class CoreAuthHandler
        : AuthenticationHandler<CoreAuthSchemeOptions>
    {
        private readonly IJwtService _jwtService;

        public static readonly string SchemeName = "CoreAuthentication";

        public CoreAuthHandler(
            IOptionsMonitor<CoreAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IJwtService jwtService)
            : base(options, logger, encoder, clock)
        {
            _jwtService = jwtService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));
            }

            var token = Request.Headers[HeaderNames.Authorization].ToString();
            var claimsPrincipal = _jwtService.ValidateToken(token);

            if (claimsPrincipal == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Token Invalid"));

            }
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            
        }
    }
}