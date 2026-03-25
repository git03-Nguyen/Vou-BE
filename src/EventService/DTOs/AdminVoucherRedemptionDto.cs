namespace EventService.DTOs;

public class AdminVoucherRedemptionDto
{
    public string VoucherToPlayerId { get; set; }
    public string EventId { get; set; }
    public string EventName { get; set; }
    public string VoucherId { get; set; }
    public string VoucherTitle { get; set; }
    public string CounterPartId { get; set; }
    public string CounterPartName { get; set; }
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
    public string? PlayerUserName { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public DateTime? UsedDate { get; set; }
    public string? UsedBy { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsExpired { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRedeemedByCounterPart { get; set; }
    public string Status { get; set; }
}
