using Shared.Data;
using Shared.Domain;

namespace GameService.Data.Models;

public class VoucherInGameSession : BaseEntity
{
    public string GameSessionId { get; set; }
    public string VoucherId { get; set; }
    
    // For Shake game
    public int? Possibility { get; set; } // % to win
    
    // For Quiz game
    public int? TopFrom { get; set; }
    public int? TopTo { get; set; }
}