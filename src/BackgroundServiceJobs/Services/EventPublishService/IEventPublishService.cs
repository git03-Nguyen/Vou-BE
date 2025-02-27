using Shared.Contracts;

namespace BackgroundServiceJobs.Services.EventPublishService;

public interface IEventPublishService
{
    Task PublishNotificationsAsync(IEnumerable<NotificationDto> notifications, CancellationToken cancellationToken);
}