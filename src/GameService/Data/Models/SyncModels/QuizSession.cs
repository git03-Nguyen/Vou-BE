namespace GameService.Data.Models.SyncModels;

public class QuizSession
{
    public string Id { get; set; }
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    public string QuizSetId { get; set; }
    public uint TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
    public int? SingleQuizTime { get; set; } = 10;
    public int? BreakTime { get; set; } = 3;
} 