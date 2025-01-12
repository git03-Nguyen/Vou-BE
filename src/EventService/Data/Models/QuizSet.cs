using System.ComponentModel.DataAnnotations.Schema;
using Shared.Contracts;
using Shared.Domain;

namespace EventService.Data.Models;

public class QuizSet : BaseEntity
{
    public string CounterPartId { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<Quiz> Quizes { get; set; }
}