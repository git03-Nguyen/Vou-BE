using Shared.Contracts.EventMessages;

namespace AuthServer.Services.PubSubService;

public interface IEventPublishService
{
    Task PublishUserUpdatedEventAsync(UserUpdatedEvent userUpdatedEvent, CancellationToken cancellationToken);

}