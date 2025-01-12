namespace EventService.DTOs.InputDtos;

public class QuizSessionInputDto
{
    public string VoucherId { get; set; }
    public string QuizSetId { get; set; }
    public int TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
}