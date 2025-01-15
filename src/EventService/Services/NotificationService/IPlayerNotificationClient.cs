using Shared.Contracts;

namespace EventService.Services.NotificationService;

public interface IPlayerNotificationClient
{
    Task NewNotification(NotificationDto notificationDto);
}