using BackgroundServiceJobs.Data.Contexts;
using BackgroundServiceJobs.Data.Models;
using BackgroundServiceJobs.Repositories.Interfaces;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories.Implements;

public class QuizSessionRepository : GenericRepository<EventDbContext, QuizSession>, IQuizSessionRepository
{
    public QuizSessionRepository(EventDbContext context) : base(context)
    {
    }
}