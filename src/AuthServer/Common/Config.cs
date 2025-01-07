using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace AuthServer.Common;

public static class Config
{
    public static IEnumerable<ApiResource> ApiResources =>
    [
        new()
        {
            Name = "api_resources",
            UserClaims =
            {
                JwtClaimTypes.Id,
                JwtClaimTypes.Name,
                JwtClaimTypes.Email,
                JwtClaimTypes.Role
            },
            Enabled = true,
            DisplayName = "Full access API",
            Scopes =
            {
                "service_api",
                IdentityServerConstants.StandardScopes.OfflineAccess
            }
        }
    ];

    // ApiScope is used to protect the API 
    //The effect is the same as that of API resources in IdentityServer 3.x
    // The difference is that API resources are more detailed and can be used to define the API's user claims
    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("service_api", "Service API"),
    ];

    // Identity resources are data like user ID, name, or email address of a user
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Email(),
        new("role", "Role", [JwtClaimTypes.Role])
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
            ClientSecrets = { new Secret("secret".Sha256()) },
            AccessTokenLifetime = 604800, // 7 days
            AllowedScopes =
            {
                "service_api"
            },
            AllowOfflineAccess = true
        }
    ];
}