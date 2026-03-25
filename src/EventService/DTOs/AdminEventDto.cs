using Shared.Enums;

namespace EventService.DTOs;

public class AdminEventDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public CounterPartDto? CounterPart { get; set; }
    public int VoucherCount { get; set; }
    public int IssuedVoucherCount { get; set; }
    public int RedeemedVoucherCount { get; set; }
    public bool HasShakeGame { get; set; }
    public int QuizSessionCount { get; set; }
}
