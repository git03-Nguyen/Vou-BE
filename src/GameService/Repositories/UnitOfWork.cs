using GameService.Data.Contexts;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories;

public class UnitOfWork : GenericUnitOfWork<GameDbContext>, IUnitOfWork
{
    public UnitOfWork
    (
        GameDbContext dbContext,
        IPlayerRepository playerRepository,
        IPlayerQuizSessionRepository playerQuizSessionRepository,
        IPlayerShakeSessionRepository playerShakeSessionRepository,
        IQuizSessionRepository quizSessionRepository,
        IQuizSetRepository quizSetRepository,
        IEventRepository eventRepository
    ) : base(dbContext)
    {
        Players = playerRepository;
        PlayerQuizSessions = playerQuizSessionRepository;
        PlayerShakeSessions = playerShakeSessionRepository;
        QuizSessions = quizSessionRepository;
        QuizSets = quizSetRepository;
        Events = eventRepository;
    }

    public IPlayerRepository Players { get; set; }
    public IPlayerQuizSessionRepository PlayerQuizSessions { get; set; }
    public IPlayerShakeSessionRepository PlayerShakeSessions { get; set; }
    public IQuizSessionRepository QuizSessions { get; set; }
    public IQuizSetRepository QuizSets { get; set; }
    public IEventRepository Events { get; set; }
}