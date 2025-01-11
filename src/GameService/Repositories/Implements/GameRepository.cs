using GameService.Data.Contexts;
using GameService.Data.Models;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class GameRepository : GenericRepository<GameDbContext, Game>, IGameRepository
{
    public GameRepository(GameDbContext context) : base(context)
    {
    }
}