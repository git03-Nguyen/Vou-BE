using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace AuthServer.Common;

public static class AuthConfig
{
    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("service_scope", "Service Scope"),
        new(IdentityServerConstants.StandardScopes.OfflineAccess)
    ];
    
    public static IEnumerable<ApiResource> ApiResources =>
    [
        new("service_api", "Service API")
        {
            Scopes = { "service_scope" },
        }
    ];

    // Identity resources are data like user ID, name, or email address of a user
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
    ];

    // Clients are applications that can access your resources, such as web applications, mobile apps, or microservices
    public static IEnumerable<Client> Clients =>
    [
        // Password flow
        new()
        {
            ClientId = "pwd.client",
            ClientName = "Password-Flow Client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("my_very_very_very_very_long_long_secret".Sha256()) },
            AccessTokenLifetime = 604800, // 7 days
            AccessTokenType = AccessTokenType.Jwt,
            AllowOfflineAccess = true,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            RefreshTokenExpiration = TokenExpiration.Absolute,
            AbsoluteRefreshTokenLifetime = 1209600, // 14 days
            AllowedScopes =
            {
                "service_scope"
            },
            UpdateAccessTokenClaimsOnRefresh = true
        }
    ];
}