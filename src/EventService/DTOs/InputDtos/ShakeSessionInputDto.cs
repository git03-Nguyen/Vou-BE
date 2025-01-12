namespace EventService.DTOs.InputDtos;

public class ShakeSessionInputDto
{
    public string VoucherId { get; set; }
    public long Price { get; set; }
    public int WinRate { get; set; }
    public int AverageDiamond { get; set; }
}