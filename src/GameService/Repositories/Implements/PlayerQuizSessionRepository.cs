using GameService.Data.Contexts;
using GameService.Data.Models;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class PlayerQuizSessionRepository : GenericRepository<GameDbContext, PlayerQuizSession>, IPlayerQuizSessionRepository
{
    public PlayerQuizSessionRepository(GameDbContext context) : base(context)
    {
    }
}