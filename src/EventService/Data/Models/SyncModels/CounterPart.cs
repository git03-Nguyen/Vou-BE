namespace EventService.Data.Models.SyncModels;

public class CounterPart
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Field { get; set; }
    public string Address { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsBlocked { get; set; }
}