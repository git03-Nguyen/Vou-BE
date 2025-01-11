using EventService.Enums;

namespace EventService.DTOs;

public class EventDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Pending;
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public CounterPartDto? CounterPart { get; set; }
}