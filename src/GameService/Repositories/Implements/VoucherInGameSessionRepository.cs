using GameService.Data.Contexts;
using GameService.Data.Models;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class VoucherInGameSessionRepository : GenericRepository<GameDbContext, VoucherInGameSession>, IVoucherInGameSessionRepository
{
    public VoucherInGameSessionRepository(GameDbContext context) : base(context)
    {
    }
}