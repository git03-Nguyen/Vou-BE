using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class QuizSetRepository : GenericRepository<EventDbContext, QuizSet>, IQuizSetRepository
{
    public QuizSetRepository(EventDbContext context) : base(context)
    {
    }
}