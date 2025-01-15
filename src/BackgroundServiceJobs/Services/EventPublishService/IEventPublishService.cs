using Shared.Contracts;

namespace PaymentService.Services.EventPublishService;

public interface IEventPublishService
{
    Task PublishNotificationsAsync(IEnumerable<NotificationDto> notifications, CancellationToken cancellationToken);
}