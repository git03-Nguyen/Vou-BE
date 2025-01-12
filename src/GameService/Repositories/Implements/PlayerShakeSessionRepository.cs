using GameService.Data.Contexts;
using GameService.Data.Models;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class PlayerShakeSessionRepository : GenericRepository<GameDbContext, PlayerShakeSession>, IPlayerShakeSessionRepository
{
    public PlayerShakeSessionRepository(GameDbContext context) : base(context)
    {
    }
}