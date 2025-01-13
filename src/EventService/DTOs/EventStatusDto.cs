using Shared.Enums;

namespace EventService.DTOs;

public class EventStatusDto
{
    public string Id { get; set; }
    public EventStatus Status { get; set; }
}