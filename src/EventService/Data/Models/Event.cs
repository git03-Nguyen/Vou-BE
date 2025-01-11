using EventService.Enums;
using Shared.Domain;

namespace EventService.Data.Models;

public class Event : BaseEntity
{
    public string Name { get; set; }
    public string CounterPartId { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; }
    
    public bool IsActive() => Status == EventStatus.InProgress;
}