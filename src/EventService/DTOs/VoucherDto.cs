namespace EventService.DTOs;

public class VoucherDto
{
    public string Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public int? TotalQuantity { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public string? RedemptionInstructions { get; set; }
    public int IssuedQuantity { get; set; }
    public int UsedQuantity { get; set; }
    public int RemainingQuantity { get; set; }
}
