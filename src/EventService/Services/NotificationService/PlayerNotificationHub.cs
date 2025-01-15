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

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation($"{HubName} User connected: {userId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, userId ?? string.Empty, Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, $"{HubName} Has error: {exception.Message}");
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier ?? string.Empty, Context.ConnectionAborted);
        var userId = Context.UserIdentifier;
        _logger.LogInformation($"{HubName} User disconnected: {userId}");
        await base.OnDisconnectedAsync(exception);
    }
}