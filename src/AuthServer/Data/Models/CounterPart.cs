using Shared.Domain;

namespace AuthServer.Data.Models;

public class CounterPart : BaseEntity
{
    public string Field { get; set; }
    public string Addresses { get; set; }
}