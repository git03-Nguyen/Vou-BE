using GameService.Data.Contexts;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories;

public class UnitOfWork : GenericUnitOfWork<GameDbContext>, IUnitOfWork
{
    public UnitOfWork(GameDbContext dbContext) : base(dbContext)
    {
    }

    public IGameRepository Games { get; }
    public IGameSessionRepository GameSessions { get; }
    public IVoucherInGameSessionRepository VoucherInGameSessions { get; }
}