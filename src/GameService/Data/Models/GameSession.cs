using System.ComponentModel.DataAnnotations.Schema;
using Shared.Data;
using Shared.Domain;

namespace GameService.Data.Models;

public class GameSession : BaseEntity
{
    public string EventId { get; set; }
    
    // For shake game session, up to one created per event, and Start time is the same as the Event's values
    // For quiz game session, multiple can be created per event, and Start time is different from each other 
    public string GameId { get; set; }
    public DateTime StartTime { get; set; }
    
    // Configuration for shake game
    public int? ShakeIntensity { get; set; } // i.e: intensity/number of shake
    
    // Configuration for quiz game
    public int? TotalQuiz { get; set; } = 10;
    public int? SingleQuizTime { get; set; } = 10; // in seconds
    public int? BreakTime { get; set; } = 3; // in seconds
    [Column(TypeName = "jsonb")]
    public Question[]? Questions { get; set; }
}