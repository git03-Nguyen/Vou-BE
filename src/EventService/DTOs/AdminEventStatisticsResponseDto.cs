namespace EventService.DTOs;

public class AdminEventStatisticsResponseDto
{
    public int TotalEvents { get; set; }
    public int TotalPendingEvents { get; set; }
    public int TotalApprovedEvents { get; set; }
    public int TotalInProgressEvents { get; set; }
    public int TotalFinishedEvents { get; set; }
    public int TotalCanceledEvents { get; set; }
    public int TotalCounterParts { get; set; }
    public int TotalPlayers { get; set; }
    public int TotalVouchers { get; set; }
    public int TotalIssuedVouchers { get; set; }
    public int TotalRedeemedVouchers { get; set; }
    public int TotalAvailableVoucherStock { get; set; }
    public int TotalEventsWithShakeGame { get; set; }
    public int TotalQuizSessions { get; set; }
}
