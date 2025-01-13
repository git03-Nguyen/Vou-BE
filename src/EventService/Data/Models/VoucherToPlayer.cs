using Shared.Domain;

namespace EventService.Data.Models;

public class VoucherToPlayer : BaseEntity
{
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    public string PlayerId { get; set; }
    public string Description { get; set; }
    public DateTime ExpiredDate { get; set; }
    public DateTime? UsedDate { get; set; }
    public string? UsedBy { get; set; }

    public bool IsUsed() => UsedDate != null;
}