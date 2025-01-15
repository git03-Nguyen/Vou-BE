using Dapr.Client;
using Shared.Common;
using Shared.Contracts;
using Shared.Contracts.EventMessages;

namespace BackgroundServiceJobs.Services.EventPublishService;

public class EventPublishService : IEventPublishService
{
    private readonly ILogger<EventPublishService> _logger;
    private readonly DaprClient _daprClient;
    public EventPublishService(ILogger<EventPublishService> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }
    
    public async Task PublishNotificationsAsync(IEnumerable<NotificationDto> notifications, CancellationToken cancellationToken)
    {
        const string methodName = $"{nameof(EventPublishService)}.{nameof(PublishNotificationsAsync)} =>";
        _logger.LogInformation(methodName);
        
        try
        {
            var message = new NotificationsSentEvent
            {
                Notifications = notifications
            };
            await _daprClient.PublishEventAsync(Constants.PubSubName, nameof(NotificationsSentEvent), message, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} Has error: {e.Message}");
        }
    }
}