using Dapr.Client;
using Shared.Common;
using Shared.Contracts.EventMessages;

namespace AuthServer.Services.PubSubService;

public class EventPublishService : IEventPublishService
{
    private readonly ILogger<EventPublishService> _logger;
    private readonly DaprClient _daprClient;
    public EventPublishService(ILogger<EventPublishService> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task PublishUserUpdatedEventAsync(UserUpdatedEvent userUpdatedEvent, CancellationToken cancellationToken)
    {
        const string methodName = $"{nameof(EventPublishService)}.{nameof(PublishUserUpdatedEventAsync)} =>";
        _logger.LogInformation(methodName);
        
        try
        {
            await _daprClient.PublishEventAsync(Constants.PubSubName, nameof(UserUpdatedEvent), userUpdatedEvent);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} Has error: {e.Message}");
        }
    }
}