namespace EventService.DTOs;

public class CounterPartDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Field { get; set; }
    public string Addresses { get; set; }
    public string? ImageUrl { get; set; }
}