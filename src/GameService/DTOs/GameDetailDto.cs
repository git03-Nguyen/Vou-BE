using GameService.Enums;

namespace GameService.DTOs;

public class GameDetailDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public GameType Type { get; set; }
}