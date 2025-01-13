using EventService.DTOs;

namespace EventService.Services.NotificationService;

public interface IPlayerNotificationClient
{
    Task NewNotification(NotificationDto notificationDto);
}