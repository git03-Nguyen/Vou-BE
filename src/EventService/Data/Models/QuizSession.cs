using Shared.Domain;

namespace EventService.Data.Models;

public class QuizSession : BaseEntity
{
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    public string QuizSetId { get; set; }
    public uint TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
}