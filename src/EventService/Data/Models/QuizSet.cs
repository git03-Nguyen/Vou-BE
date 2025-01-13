using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Shared.Contracts;
using Shared.Domain;

namespace EventService.Data.Models;

public class QuizSet : BaseEntity
{
    public string CounterPartId { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    
    [Column(TypeName = "json")]
    public string QuizesSerialized { get; set; }
}