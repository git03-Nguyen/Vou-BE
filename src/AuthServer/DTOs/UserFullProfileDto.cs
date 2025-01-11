using System.Text.Json.Serialization;
using AuthServer.Data.Models;
using AuthServer.DTOs.Abstraction;
using Shared.Contracts;

namespace AuthServer.DTOs;

public class UserFullProfileDto : BaseUserProfileDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ProfileLinked? ProfileLinked { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedDate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ModifiedDate { get; set; }
    
    public bool IsBlocked { get; set; }
    public DateTime? BlockedDate { get; set; }
    
    // For CounterPart
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Addresses { get; set; }
    
    // For Player
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? BirthDate { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Gender? Gender { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FacebookUrl { get; set; }
}