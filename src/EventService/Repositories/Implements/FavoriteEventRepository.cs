using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class FavoriteEventRepository : GenericRepository<EventDbContext, FavoriteEvent>, IFavoriteEventRepository
{
    public FavoriteEventRepository(EventDbContext context) : base(context)
    {
    }
}