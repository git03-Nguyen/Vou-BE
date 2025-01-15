using PaymentService.Data.Contexts;
using PaymentService.Data.Models;
using PaymentService.Repositories.Interfaces;
using Shared.Repositories;

namespace PaymentService.Repositories.Implements;

public class QuizSessionRepository : GenericRepository<EventDbContext, QuizSession>, IQuizSessionRepository
{
    public QuizSessionRepository(EventDbContext context) : base(context)
    {
    }
}