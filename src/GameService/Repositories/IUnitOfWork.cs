using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IGameRepository Games { get; }
    IGameSessionRepository GameSessions { get; }
    IVoucherInGameSessionRepository VoucherInGameSessions { get; }
}