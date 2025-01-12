using GameService.Data.Contexts;
using GameService.Data.Models.SyncModels;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class QuizSetRepository : GenericRepository<GameDbContext, QuizSet>, IQuizSetRepository
{
    public QuizSetRepository(GameDbContext context) : base(context)
    {
    }
}