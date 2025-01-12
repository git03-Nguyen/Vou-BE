using EventService.Enums;

namespace EventService.DTOs;

public class FullEventDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public EventStatus Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public ShakeSessionDto? ShakeSession { get; set; }
    public List<QuizSessionDto>? QuizSessions { get; set; }
}