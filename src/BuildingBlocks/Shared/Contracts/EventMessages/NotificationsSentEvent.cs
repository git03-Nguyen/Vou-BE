namespace Shared.Contracts.EventMessages;

public class NotificationsSentEvent
{
    public IEnumerable<NotificationDto> Notifications { get; set; }
}