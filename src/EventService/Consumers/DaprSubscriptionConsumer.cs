using System.Text.Json;
using Dapr;
using Dapr.AspNetCore;
using EventService.Data.Models.SyncModels;
using EventService.Repositories;
using EventService.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Common;
using Shared.Contracts.EventMessages;

namespace EventService.Consumers;

[ApiController]
[Route("dapr/[controller]")]
public class DaprSubscriptionConsumer : ControllerBase
{
    private readonly ILogger<DaprSubscriptionConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<PlayerNotificationHub, IPlayerNotificationClient> _playerNotificationHub;
    public DaprSubscriptionConsumer(ILogger<DaprSubscriptionConsumer> logger, IUnitOfWork unitOfWork, IHubContext<PlayerNotificationHub, IPlayerNotificationClient> playerNotificationHub)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _playerNotificationHub = playerNotificationHub;
    }

    [HttpPost($"{nameof(UserUpdatedEvent)}_Route")]
    [Topic(Constants.PubSubName, nameof(UserUpdatedEvent), $"{nameof(UserUpdatedEvent)}_DeadLetter", false)]
    [BulkSubscribe(nameof(UserUpdatedEvent), 500, 2000)]
    public async Task<IActionResult> HandleUserUpdatedAsync
    (
        [FromBody] BulkSubscribeMessage<BulkMessageModel<UserUpdatedEvent>> bulkMessage,
        CancellationToken cancellationToken
    )
    {
        List<CounterPart>? counterPartsToInsert = null;
        List<Player>? playersToInsert = null;
        foreach (var entry in bulkMessage.Entries)
        {
            var message = entry.Event.Data;
            var methodName = $"{nameof(DaprSubscriptionConsumer)}.{nameof(HandleUserUpdatedAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
            _logger.LogInformation(methodName);

            try
            {
                // CounterPart
                if (message.Role == Constants.COUNTERPART)
                {
                    var counterPart = new CounterPart
                    {
                        Id = message.UserId,
                        FullName = message.FullName,
                        Field = message?.Field ?? string.Empty,
                        Address = message?.Addresses ?? string.Empty,
                        ImageUrl = message?.AvatarUrl ?? Constants.DefaultAvatar,
                        IsBlocked = false
                    };
                    
                    counterPartsToInsert ??= [];
                    counterPartsToInsert.Add(counterPart);
                }
                // Player
                else if (message.Role == Constants.PLAYER)
                {
                    var player = new Player
                    {
                        Id = message.UserId,
                        FullName = message.FullName,
                        Email = message.Email,
                        PhoneNumber = message.PhoneNumber,
                        ImageUrl = message?.AvatarUrl ?? Constants.DefaultAvatar,
                        UserName = message?.UserName ?? string.Empty
                    };

                    playersToInsert ??= new List<Player>();
                    playersToInsert.Add(player);
                }

                if (playersToInsert is not null)
                {
                    await _unitOfWork.Players.AddRangeAsync(playersToInsert, cancellationToken);
                }

                if (counterPartsToInsert is not null)
                {
                    await _unitOfWork.CounterParts.AddRangeAsync(counterPartsToInsert, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"{methodName} Has error: {e.Message}");
                return BadRequest();
            }
        }
        
        return Ok();
    }
    
    [HttpPost($"{nameof(NotificationsSentEvent)}_Route")]
    [Topic(Constants.PubSubName, nameof(NotificationsSentEvent), $"{nameof(NotificationsSentEvent)}_DeadLetter", false)]
    [BulkSubscribe(nameof(NotificationsSentEvent), 500, 2000)]
    public async Task<IActionResult> HandleNotificationsSentAsync
    (
        [FromBody] BulkSubscribeMessage<BulkMessageModel<NotificationsSentEvent>> bulkMessage,
        CancellationToken cancellationToken
    )
    {
        foreach (var entry in bulkMessage.Entries)
        {
            var message = entry.Event.Data;
            var methodName = $"{nameof(DaprSubscriptionConsumer)}.{nameof(HandleNotificationsSentAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
            _logger.LogInformation(methodName);

            foreach (var notif in message.Notifications)
            {
                try
                {
                    await _playerNotificationHub.Clients.Group(notif.PlayerId).NewNotification(notif);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{methodName} PlayerId = {notif.PlayerId}, Has error: {e.Message}");
                }
            }
        }
        
        return Ok();
    }
}