using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Refit;
using WebClient.Infrastructure;
using WebClient.Mapping;
using WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
AddCustomAuthentication();

services.AddMvc(options => options.EnableEndpointRouting = false);

services.AddAutoMapper(conf => conf.AddProfile<MappingProfile>());

var calendarUrl = configuration.GetValue<string>("CalendarUrl");

services.AddTransient<ICalendarService, CalendarService>();

services.AddRefitClient<ICalendarApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{calendarUrl}/api/v1/Events"))
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
    .AddPolicyHandlers();

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseMvcWithDefaultRoute();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    app.Run();
}
catch (Exception e)
{
    logger.LogCritical(e, "Fatal exception.");
}


void AddCustomAuthentication()
{
    var identityUrl = configuration.GetValue<string>("IdentityUrl");

    services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(60))
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.ClientId = "mvc";
            options.ClientSecret = "secret";
            options.ResponseType = "code id_token";
            options.SaveTokens = true;
            options.Scope.Add("api");
            options.Scope.Add("offline_access");
            options.UseTokenLifetime = true;

            options.Events = new OpenIdConnectEvents
            {
                OnRemoteFailure = CustomHandlers.HandleCancelAction,
                OnTokenResponseReceived = CustomHandlers.CopyAllowedScopesToUserClaims
            };
        });

    services.AddAuthorization();
}


internal static class HttpClientBuilderExtensions
{
    internal static IHttpClientBuilder AddPolicyHandlers(this IHttpClientBuilder builder)
    {
        builder
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .RetryAsync(3))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(10));

        return builder;
    }
}


internal static class CustomHandlers
{
    public static Task CopyAllowedScopesToUserClaims(TokenResponseReceivedContext context)
    {
        var scopes = context.ProtocolMessage.Scope?.Split(' ');
        if (scopes != null && context.Principal?.Identity is ClaimsIdentity identity)
        {
            foreach (var scope in scopes)
            {
                identity.AddClaim(new Claim("scope", scope));
            }
        }
        return Task.CompletedTask;
    }

    public static Task HandleCancelAction(RemoteFailureContext context)
    {
        context.Response.Redirect("/");
        context.HandleResponse();
        return Task.CompletedTask;
    }
}
