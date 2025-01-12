using GameService.Data.Contexts;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories;

public class UnitOfWork : GenericUnitOfWork<GameDbContext>, IUnitOfWork
{
    public UnitOfWork(GameDbContext dbContext) : base(dbContext)
    {
    }

    public IPlayerRepository Players { get; set; }
    public IPlayerQuizSessionRepository PlayerQuizSessions { get; set; }
    public IPlayerShakeSessionRepository PlayerShakeSessions { get; set; }
    public IQuizSessionRepository QuizSessions { get; set; }
    public IQuizSetRepository QuizSets { get; set; }
}