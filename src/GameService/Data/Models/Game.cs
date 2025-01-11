using GameService.Enums;
using Shared.Domain;

namespace GameService.Data.Models;

public class Game : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public GameType Type { get; set; }
}