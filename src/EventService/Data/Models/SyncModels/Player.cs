namespace EventService.Data.Models.SyncModels;

public class Player
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
}