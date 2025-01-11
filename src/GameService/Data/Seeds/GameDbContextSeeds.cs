using GameService.Data.Models;
using GameService.Enums;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace GameService.Data.Seeds;

public static class GameDbContextSeeds
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<Game>().HasData(
            // Shake Game
            new Game
            {
                Id = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                Name = "Shake Game",
                Description = "This is shake game",
                Author = "TuanDat-ThienAn",
                CreatedBy = Constants.SYSTEM,
                Type = GameType.Shake,
                ImageUrl = Constants.DefaultGameImageUrl,
            },
            // Quiz Game
            new Game
            {
                Id = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                Name = "Quiz Game",
                Description = "This is quiz game",
                Author = "TuanDat-ThienAn",
                CreatedBy = Constants.SYSTEM,
                Type = GameType.Quiz,
                ImageUrl = Constants.DefaultGameImageUrl,
            }
        );        
    }
}