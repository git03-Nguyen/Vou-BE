using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Contracts;

namespace EventService.DTOs;

public class QuizSetDto
{
    public string Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public List<Quiz> Quizes => (QuizesSerialized is null 
                                      ? [] 
                                      : JsonSerializer.Deserialize<List<Quiz>>(QuizesSerialized)) 
                                  ?? [];
    [JsonIgnore]
    public string? QuizesSerialized { get; set; }
}