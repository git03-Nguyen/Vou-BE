using Shared.Domain;

namespace EventService.Data.Models;

public class VoucherInEvent : BaseEntity
{
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    
    // For shake game
    public long Price { get; set; }
}