namespace EventService.DTOs;

public class ShakeSessionDto
{
    public VoucherDto? Voucher { get; set; }
    public ulong Price { get; set; }
    public uint WinRate { get; set; }
    public uint AverageDiamond { get; set; }
}