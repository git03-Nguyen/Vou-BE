using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Common;

namespace EventService.Services.NotificationService;

[Authorize(Policy = Constants.PLAYER)]
public class PlayerNotificationHub : Hub<IPlayerNotificationClient>
{
    private readonly ILogger<PlayerNotificationHub> _logger;
    private const string HubName = $"{nameof(PlayerNotificationHub)} =>";
    public PlayerNotificationHub(ILogger<PlayerNotificationHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation($"{HubName} User connected: {userId}");
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation($"{HubName} User disconnected: {userId}");
        return base.OnDisconnectedAsync(exception);
    }
}