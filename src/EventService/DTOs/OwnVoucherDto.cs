namespace EventService.DTOs;

public class OwnVoucherDto
{
    public string EventId { get; set; }
    public int Count { get; set; }
    public VoucherDto Voucher { get; set; }
}