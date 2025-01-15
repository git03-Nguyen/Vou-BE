using Shared.Domain;
using Shared.Enums;

namespace BackgroundServiceJobs.Data.Models;

public class QuizSession : BaseEntity
{
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    public string QuizSetId { get; set; }
    public int TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
    public QuizSessionStatus Status { get; set; } = QuizSessionStatus.Pending;
}