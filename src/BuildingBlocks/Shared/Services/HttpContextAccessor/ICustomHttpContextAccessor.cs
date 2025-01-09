namespace Shared.Services.HttpContextAccessor;

public interface ICustomHttpContextAccessor
{
    bool IsAuthenticated();
    string GetCurrentUserId();
    string GetCurrentRole();
    string GetCurrentEmail();
    string GetCurrentUserName();
    string GetCurrentIpAddress();
    string GetCurrentJwtToken();
}