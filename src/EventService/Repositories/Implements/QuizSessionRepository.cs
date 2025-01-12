using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class QuizSessionRepository : GenericRepository<EventDbContext, QuizSession>, IQuizSessionRepository
{
    public QuizSessionRepository(EventDbContext context) : base(context)
    {
    }
}