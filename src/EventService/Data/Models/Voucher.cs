using Shared.Domain;

namespace EventService.Data.Models;

public class Voucher : BaseEntity
{
    public string CounterPartId { get; set; }
    public string ImageUrl { get; set; }
    public int Value { get; set; }
    public string Description { get; set; }
}