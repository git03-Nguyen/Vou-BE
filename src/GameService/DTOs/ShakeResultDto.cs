namespace GameService.DTOs;

public class ShakeResultDto
{
    public string PlayerId { get; set; }
    public int ReceivedDiamonds { get; set; }
    public int TotalDiamonds { get; set; }
    public int TotalTickets { get; set; }
}