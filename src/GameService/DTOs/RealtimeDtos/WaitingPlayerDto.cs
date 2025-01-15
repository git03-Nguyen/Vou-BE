using System.Text.Json.Serialization;

namespace GameService.DTOs.RealtimeDtos;

public class WaitingPlayerDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = "Unknown Player";
    
    [JsonIgnore]
    public string ConnectionId { get; set; } = string.Empty;
    [JsonIgnore]
    public int Score { get; set; }
}