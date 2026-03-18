namespace EventService.DTOs;

public class OwnVoucherDto
{
    public string EventId { get; set; }
    public int Count { get; set; }
    public int AvailableCount { get; set; }
    public int UsedCount { get; set; }
    public int ExpiredCount { get; set; }
    public VoucherDto Voucher { get; set; }
    public List<OwnVoucherItemDto> Items { get; set; } = new();
}

public class OwnVoucherItemDto
{
    public string VoucherToPlayerId { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public DateTime? UsedDate { get; set; }
    public string? UsedBy { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsExpired { get; set; }
    public bool IsRedeemedByCounterPart { get; set; }
    public string Status { get; set; }
}
