namespace EventService.DTOs;

public class UseVoucherDto
{
    public string Id { get; set; }
    public string VoucherId { get; set; }
    public string PlayerId { get; set; }
    public string? UsedBy { get; set; }
    public DateTime UsedDate { get; set; }
}
