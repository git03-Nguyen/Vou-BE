namespace EventService.Data.Models.SyncModels;

public class CounterPart
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Field { get; set; }
    public string Addresses { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsBlocked { get; set; }
}