using GdeBabki.Server.Services;
using GdeBabki.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GdeBabki.Server.Auth
{
    public class GBAuthenticationHandler : AuthenticationHandler<GBAuthenticationOptions>
    {
        private readonly DatabaseService databaseService;

        public GBAuthenticationHandler(IOptionsMonitor<GBAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, 
            DatabaseService databaseService) 
            : base (options, logger, encoder, clock)
        {
            this.databaseService = databaseService;
        }

        const string UNAUTHORIZED = "Unauthorized";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userHasDbAccess = await databaseService.UserHasDbAccess();
            if (!userHasDbAccess)
            {
                return AuthenticateResult.Fail(UNAUTHORIZED);
            }

            var loginInfo = GBAuthentication.GetLoginInfoFromAuthHeader(Request.Headers.Authorization);
            if (loginInfo == null)
            {
                return AuthenticateResult.Fail(UNAUTHORIZED);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginInfo.UserName)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new GenericPrincipal(identity, null);            
            var ticket = new AuthenticationTicket(principal, Scheme.Name);            

            return AuthenticateResult.Success(ticket);
        }
    }
}
