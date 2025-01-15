namespace EventService.DTOs;

public class CounterPartStatisticsEventsResponseDto
{
    public int TotalEvents { get; set; }
    public int TotalVouchers { get; set; }
    
    public int TotalVouchersValue{ get; set; }
    
    public int TotalQuizSessions { get; set; }
    public int TotalShakeGame { get; set; }
}