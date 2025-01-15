using Shared.Enums;

namespace GameService.Data.Models.SyncModels;

public class Event
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CounterPartId { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public EventStatus Status { get; set; } = EventStatus.InProgress; // Default is InProgress
    
    // For Shake game
    public string? ShakeVoucherId { get; set; }
    public long? ShakePrice { get; set; } = 250;
    public int? ShakeWinRate { get; set; } = 80; // 80%
    public int? ShakeAverageDiamond { get; set; } = 50; // 50 diamonds
}