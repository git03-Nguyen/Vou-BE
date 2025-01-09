using GameService.Data.Contexts;
using GameService.Data.Models;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class GameSessionRepository : GenericRepository<GameDbContext, GameSession>, IGameSessionRepository
{
    public GameSessionRepository(GameDbContext context) : base(context)
    {
    }
}