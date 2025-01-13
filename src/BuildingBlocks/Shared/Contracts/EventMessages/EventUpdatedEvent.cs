using Shared.Enums;

namespace Shared.Contracts.EventMessages;

public class EventUpdatedEvent
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? CounterPartId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EventStatus? Status { get; set; }
    
    // For Shake game
    public string? ShakeVoucherId { get; set; }
    public long? ShakePrice { get; set; } = 250;
    public int? ShakeWinRate { get; set; } = 80; // 80%
    public int? ShakeAverageDiamond { get; set; } = 50; // 50 diamonds
}