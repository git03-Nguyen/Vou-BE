using System.Text.Json.Serialization;

namespace EventService.DTOs;

public class QuizSessionDto
{
    public string Id { get; set; }
    [JsonIgnore]
    public string EventId { get; set; }
    public VoucherDto? Voucher { get; set; }
    public QuizSetDto QuizSet { get; set; }
    public int TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
}