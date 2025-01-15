namespace Shared.Contracts.EventMessages;

public class EventCreatedNotifier
{
    public string EventId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}