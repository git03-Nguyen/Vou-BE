using Shared.Domain;

namespace PaymentService.Data.Models;

public class FavoriteEvent : BaseEntity
{
    public string PlayerId { get; set; }
    public string EventId { get; set; }
    public bool HasNotified { get; set; }
}