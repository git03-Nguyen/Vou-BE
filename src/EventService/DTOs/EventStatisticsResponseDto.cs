namespace EventService.DTOs;

public class EventStatisticsResponseDto
{
    public int TotalActiveEvents { get; set; }
    public int TotalVouchers { get; set; }
    public int TotalPlayers { get; set; }
    public int TotalIssuedVouchers { get; set; }
    public int TotalRedeemedVouchers { get; set; }
    public int TotalAvailableVoucherStock { get; set; }
}
