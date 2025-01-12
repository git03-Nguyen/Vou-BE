using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IPlayerRepository Players { get; set; }
    IPlayerQuizSessionRepository PlayerQuizSessions { get; set; }
    IPlayerShakeSessionRepository PlayerShakeSessions { get; set; }
    IQuizSessionRepository QuizSessions { get; set; }
    IQuizSetRepository QuizSets { get; set; }
}