namespace EventService.DTOs;

public class BuyVoucherDto
{
  public VoucherDto Voucher { get; set; }
  
  public long Diamonds { get; set; }
  public string VoucherToPlayerId { get; set; }
}