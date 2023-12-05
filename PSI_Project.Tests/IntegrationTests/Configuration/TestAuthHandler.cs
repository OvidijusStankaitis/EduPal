using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PSI_Project.Tests.IntegrationTests.Configuration;

public class TestAuthHandler : AuthenticationHandler<TestAuthOptions>
{
    
    public TestAuthHandler(IOptionsMonitor<TestAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
        }, "TestScheme");

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}