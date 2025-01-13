namespace Shared.Contracts.EventMessages;

public class UserUpdatedEvent
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; }
    
    // For Player
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? FacebookUrl { get; set; }
    
    // For CounterPart
    public string? Field { get; set; }
    public string? Addresses { get; set; }
}