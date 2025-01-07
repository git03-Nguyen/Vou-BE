using System.Security.Claims;
using AuthServer.Data.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Services.ProfileService;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    public CustomProfileService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user is null) return;
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, user.Id),
            new(JwtClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtClaimTypes.Role, roles.FirstOrDefault() ?? "USER"),
        };
        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user is not null;
    }
}