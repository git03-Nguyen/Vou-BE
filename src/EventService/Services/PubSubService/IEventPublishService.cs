using EventService.DTOs;

namespace EventService.Services.PubSubService;

public interface IEventPublishService
{
    Task PublishEventUpdatedEventAsync(FullEventDto eventUpdatedEvent, CancellationToken cancellationToken);

}