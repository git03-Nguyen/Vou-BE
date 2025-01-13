using System.Text.Json;
using Dapr;
using Dapr.AspNetCore;
using GameService.Data.Models.SyncModels;
using GameService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Contracts.EventMessages;
using Shared.Enums;

namespace GameService.Consumers;

[ApiController]
[Route("dapr/[controller]")]
public class SubscriptionConsumer : ControllerBase
{
    private readonly ILogger<SubscriptionConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public SubscriptionConsumer(ILogger<SubscriptionConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet("/dapr/subscribe")]
    public IActionResult GetSubscriptions()
    {
        return Ok(new[] 
        {
            new 
            { 
                pubsubname = Constants.PubSubName,
                topic = nameof(UserUpdatedEvent),
                route = $"{nameof(UserUpdatedEvent)}_Route"
            },
            new 
            { 
                pubsubname = Constants.PubSubName,
                topic = nameof(EventUpdatedEvent),
                route = $"{nameof(EventUpdatedEvent)}_Route"
            }
        });
    }

    [HttpPost($"{nameof(UserUpdatedEvent)}_Route")]
    [Topic(Constants.PubSubName, nameof(UserUpdatedEvent), "UserUpdatedEvent_DeadLetter", false)]
    [BulkSubscribe(nameof(UserUpdatedEvent), 500, 2000)]
    public async Task<IActionResult> HandleUserUpdatedAsync
    (
        [FromBody] BulkSubscribeMessage<BulkMessageModel<UserUpdatedEvent>> bulkMessage,
        CancellationToken cancellationToken
    )
    {
        List<Player>? playersToInsert = null;
        foreach (var entry in bulkMessage.Entries)
        {
            var message = entry.Event.Data;
            var methodName = $"{nameof(SubscriptionConsumer)}.{nameof(HandleUserUpdatedAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
            _logger.LogInformation(methodName);

            try
            {
                if (message.Role == Constants.PLAYER)
                {
                    var player = new Player
                    {
                        Id = message.UserId,
                        FullName = message.FullName,
                        Email = message.Email,
                        PhoneNumber = message.PhoneNumber,
                        AvatarUrl = message.AvatarUrl ?? Constants.DefaultAvatar,
                        UserName = message.UserName
                    };

                    playersToInsert ??= new List<Player>();
                    playersToInsert.Add(player);
                }

                if (playersToInsert is not null)
                {
                    await _unitOfWork.Players.AddRangeAsync(playersToInsert, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{methodName} Has error: {e.Message}");
                return BadRequest();
            }
        }
        
        return Ok();
    }
    
    [HttpPost($"{nameof(EventUpdatedEvent)}_Route")]
    [Topic(Constants.PubSubName, nameof(EventUpdatedEvent), $"{nameof(EventUpdatedEvent)}_DeadLetter", false)]
    [BulkSubscribe(nameof(EventUpdatedEvent), 500, 2000)]
    public async Task<IActionResult> HandleEventUpdatedAsync
    (
        [FromBody] BulkSubscribeMessage<BulkMessageModel<EventUpdatedEvent>> bulkMessage,
        CancellationToken cancellationToken
    )
    {
        List<Event>? eventsToInsert = null;
        foreach (var entry in bulkMessage.Entries)
        {
            var message = entry.Event.Data;
            var methodName = $"{nameof(SubscriptionConsumer)}.{nameof(HandleEventUpdatedAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
            _logger.LogInformation(methodName);

            try
            {
                var @event = new Event
                {
                    Id = message.Id,
                    Name = message.Name,
                    Description = message.Description,
                    ImageUrl = message.ImageUrl,
                    Status = message.Status ?? EventStatus.Canceled,
                    ShakePrice = message.ShakePrice,
                    CounterPartId = message.CounterPartId ?? string.Empty,
                    ShakeAverageDiamond = message.ShakeAverageDiamond,
                    ShakeVoucherId = message.ShakeVoucherId,
                    ShakeWinRate = message.ShakeWinRate,
                };
                
                eventsToInsert ??= [];
                eventsToInsert.Add(@event);

                await _unitOfWork.Events.AddRangeAsync(eventsToInsert, cancellationToken);
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
}