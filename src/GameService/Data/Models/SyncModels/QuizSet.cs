using System.ComponentModel.DataAnnotations.Schema;
using Shared.Contracts;

namespace GameService.Data.Models.SyncModels;

public class QuizSet
{
    public string Id { get; set; }
    public string CounterPartId { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<Quiz> Question { get; set; }
}