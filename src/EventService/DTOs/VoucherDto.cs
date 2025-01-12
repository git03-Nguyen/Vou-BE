namespace EventService.DTOs;

public class VoucherDto
{
    public string Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
}