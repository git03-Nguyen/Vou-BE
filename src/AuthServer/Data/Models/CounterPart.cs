using System.ComponentModel.DataAnnotations.Schema;
using Shared.Contract;
using Shared.Domain;

namespace AuthServer.Data.Models;

public class CounterPart : BaseEntity
{
    public string Name { get; set; }
    public string Field { get; set; }
    [Column(TypeName = "jsonb")]
    public Address[]? Addresses { get; set; }
}