using Shared.Contract;
using Shared.Domain;

namespace AuthServer.Data.Models;

public class Player : BaseEntity
{
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; } = Gender.Other;
    public string? FacebookUrl { get; set; }
    public string? DeviceId { get; set; }
}