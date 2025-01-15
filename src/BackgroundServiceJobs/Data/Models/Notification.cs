using Shared.Domain;

namespace PaymentService.Data.Models;

public class Notification : BaseEntity
{
    public string PlayerId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
}