using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<TestUser> GetUsers() =>
        new TestUser[]
        {
            new()
            {
                SubjectId = "1",
                Username = "demo",
                Password = "demo",
                Claims = new Claim[] { new ("name", "Demo") }
            }
        };

    public static IEnumerable<IdentityResource> GetIdentityResources() =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> GetApis() =>
        new ApiResource[]
        {
            new()
            {
                Name = "api",
                DisplayName = "API",
                Scopes = { "api" }
            }
        };

    public static IEnumerable<ApiScope> GetScopes() =>
        new ApiScope[]
        {
            new("api", "API")
        };

    public static IEnumerable<Client> GetClients() =>
        new Client[]
        {
            new()
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                AllowedGrantTypes = GrantTypes.Hybrid,

                ClientSecrets = { new Secret("secret".Sha256()) },

                RedirectUris =
                {
                    "http://localhost:7002/signin-oidc",
                    "http://host.docker.internal:7002/signin-oidc"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:7002/signout-callback-oidc",
                    "http://host.docker.internal:7002/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api"
                },

                AllowOfflineAccess = true,

                RequireConsent = true,
                RequirePkce = false,

                AccessTokenLifetime = (int)TimeSpan.FromHours(2).TotalSeconds,
                IdentityTokenLifetime = (int)TimeSpan.FromHours(2).TotalSeconds
            },
            new()
            {
                ClientId = "calendarswaggerui",
                ClientName = "Calendar Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris =
                {
                    "http://localhost:7001/swagger/oauth2-redirect.html",
                    "http://host.docker.internal:7001/swagger/oauth2-redirect.html"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:7001/swagger",
                    "http://host.docker.internal:7001/swagger"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api"
                },

                RequireConsent = true
            }
        };
}