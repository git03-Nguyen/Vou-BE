using BackgroundServiceJobs.Data.Contexts;
using BackgroundServiceJobs.Data.Models;
using BackgroundServiceJobs.Repositories.Interfaces;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories.Implements;

public class FavoriteEventRepository : GenericRepository<EventDbContext, FavoriteEvent>, IFavoriteEventRepository
{
    public FavoriteEventRepository(EventDbContext context) : base(context)
    {
    }
}