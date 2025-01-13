using System.Text.Json;
using Dapr;
using Dapr.AspNetCore;
using GameService.Data.Models.SyncModels;
using GameService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Contracts.EventMessages;

namespace GameService.Consumers;

[ApiController]
[Route("dapr/[controller]")]
public class DaprSubscriptionConsumer : ControllerBase
{
    private readonly ILogger<DaprSubscriptionConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public DaprSubscriptionConsumer(ILogger<DaprSubscriptionConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
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
            var methodName = $"{nameof(DaprSubscriptionConsumer)}.{nameof(HandleUserUpdatedAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
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
}