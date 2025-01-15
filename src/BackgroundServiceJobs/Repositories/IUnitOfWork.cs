using BackgroundServiceJobs.Repositories.Interfaces;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IEventRepository Events { get; set; }
    IQuizSessionRepository QuizSessions { get; set; }
    IFavoriteEventRepository FavoriteEvents { get; set; }
    INotificationRepository Notifications { get; set; }
}