using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IEventRepository Events { get; set; }
    IVoucherRepository Vouchers { get; set; }
    IQuizSessionRepository QuizSessions { get; set; }
    IQuizSetRepository QuizSets { get; set; }
    IVoucherToPlayerRepository VoucherToPlayers { get; set; }
    IFavoriteEventRepository FavoriteEvents { get; set; }
    IPlayerRepository Players { get; set; }
    ICounterPartRepository CounterParts { get; set; }
}