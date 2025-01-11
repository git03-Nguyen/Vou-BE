using System.Security.Claims;
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
    public string GetCurrentRole() => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    public string GetCurrentEmail() => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    public string GetCurrentUserName() => _httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value ?? string.Empty;
    public string GetCurrentIpAddress()
    {
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        return ip ?? string.Empty;
    }

    public string GetCurrentJwtToken()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        return token?.Replace("Bearer ", string.Empty) ?? string.Empty;
    }
}