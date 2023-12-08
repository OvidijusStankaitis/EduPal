using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PSI_Project.Services;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace PSI_Project.Tests.IntegrationTests.Configuration;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserAuthService _userAuthService;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserAuthService userAuthService)
        : base(options, logger, encoder, clock)
    {
        _userAuthService = userAuthService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = await _userAuthService.GetUser(Context);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
            };

            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return AuthenticateResult.Success(ticket);
        }
        
        return AuthenticateResult.NoResult();
    }
}