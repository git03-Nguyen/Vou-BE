using Shared.Contracts;

namespace EventService.DTOs;

public class QuizSetDto
{
    public string Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public List<Quiz> Quizes { get; set; }
}