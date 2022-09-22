using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddIdentityServer(x =>
    {
        x.IssuerUri = "null";
        x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
    })
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryApiResources(Config.GetApis())
    .AddInMemoryApiScopes(Config.GetScopes())
    .AddInMemoryClients(Config.GetClients())
    .AddTestUsers(Config.GetUsers().ToList())
    .AddDeveloperSigningCredential();

services.AddControllersWithViews();


var app = builder.Build();

app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
    await next();
});

app.UseForwardedHeaders();

app.UseIdentityServer();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});

app.Run();
