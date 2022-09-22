using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calendar.Api.FunctionalTests.Helpers;

public class FakeSchemeProvider : AuthenticationSchemeProvider
{
    public FakeSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        var scheme = new AuthenticationScheme(
            "Test",
            "Test",
            typeof(FakeAuthenticationHandler)
        );
        return Task.FromResult(scheme)!;
    }
}


public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, "Test user"),
            new("sub", "1")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}