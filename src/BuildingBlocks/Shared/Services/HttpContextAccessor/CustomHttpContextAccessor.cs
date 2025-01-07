using Microsoft.AspNetCore.Http;

namespace Shared.Services.HttpContextAccessor;

public class CustomHttpContextAccessor : ICustomHttpContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CustomHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated() => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string GetCurrentUserId() => _httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value ?? string.Empty;
    public string GetCurrentRole() => _httpContextAccessor.HttpContext?.User.FindFirst("role")?.Value ?? string.Empty;
    public string GetCurrentEmail() => _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value ?? string.Empty;
    public string GetCurrentUserName() => _httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value ?? string.Empty;
}