using GameService.Helpers;
using Shared.Domain;

namespace GameService.Data.Models;

public class PlayerShakeSession : BaseEntity
{
    public string PlayerId { get; set; }
    public string EventId { get; set; }
    public int Tickets { get; set; } = 5;
    public DateTime? LastShareTime { get; set; } 
    public DateTime? NextResetTicketsTime { get; set; } = TimeHelpers.GetNextMonday();
    public int Diamond { get; set; } = 0;
}