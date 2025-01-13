using Dapr.Client;
using EventService.DTOs;
using Shared.Common;
using Shared.Contracts.EventMessages;

namespace EventService.Services.PubSubService;

public class EventPublishService : IEventPublishService
{
    private readonly ILogger<EventPublishService> _logger;
    private readonly DaprClient _daprClient;
    public EventPublishService(ILogger<EventPublishService> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task PublishEventUpdatedEventAsync(FullEventDto fullEventDto, CancellationToken cancellationToken)
    {
        const string methodName = $"{nameof(EventPublishService)}.{nameof(PublishEventUpdatedEventAsync)} =>";
        _logger.LogInformation(methodName);
        
        try
        {
            var eventUpdatedEvent = new EventUpdatedEvent
            {
                Id = fullEventDto.Id,
                Name = fullEventDto.Name,
                Description = fullEventDto.Description,
                ImageUrl = fullEventDto.ImageUrl,
                StartDate = fullEventDto.StartDate,
                EndDate = fullEventDto.EndDate,
                Status = fullEventDto.Status,
                CounterPartId = fullEventDto.CounterPart?.Id,
                ShakePrice = fullEventDto.ShakeSession?.Price,
                ShakeAverageDiamond = fullEventDto.ShakeSession?.AverageDiamond,
                ShakeVoucherId = fullEventDto.ShakeSession?.Voucher?.Id,
                ShakeWinRate = fullEventDto.ShakeSession?.WinRate,
            };
            await _daprClient.PublishEventAsync(Constants.PubSubName, nameof(EventUpdatedEvent), eventUpdatedEvent, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} Has error: {e.Message}");
        }
    }
}