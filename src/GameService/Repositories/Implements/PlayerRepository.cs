using GameService.Data.Contexts;
using GameService.Data.Models.SyncModels;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class PlayerRepository : GenericRepository<GameDbContext, Player>, IPlayerRepository
{
    public PlayerRepository(GameDbContext context) : base(context)
    {
    }
}