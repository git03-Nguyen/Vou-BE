namespace Shared.Contracts;

public class PlayerTicketDiamondResponse
{
    public int Tickets { get; set; } = 0;
    public long Diamonds { get; set; } = 0;
    public bool IsSuccess { get; set; } = false;
}