namespace Shared.Contracts;

public class UpdatePlayerDiamondRequest
{
    public string PlayerId { get; set; }
    public string EventId { get; set; }
    public long Diamonds { get; set; }
}