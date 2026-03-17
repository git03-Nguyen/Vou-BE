using Shared.Domain;

namespace EventService.Data.Models;

public class Voucher : BaseEntity
{
    public string CounterPartId { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public int? TotalQuantity { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public string? RedemptionInstructions { get; set; }
}
