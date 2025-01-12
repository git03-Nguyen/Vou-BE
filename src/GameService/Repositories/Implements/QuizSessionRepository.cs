using GameService.Data.Contexts;
using GameService.Data.Models.SyncModels;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class QuizSessionRepository : GenericRepository<GameDbContext, QuizSession>, IQuizSessionRepository
{
    public QuizSessionRepository(GameDbContext context) : base(context)
    {
    }
}