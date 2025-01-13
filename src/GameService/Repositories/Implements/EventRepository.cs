using GameService.Data.Contexts;
using GameService.Data.Models.SyncModels;
using GameService.Repositories.Interfaces;
using Shared.Repositories;

namespace GameService.Repositories.Implements;

public class EventRepository : GenericRepository<GameDbContext, Event>, IEventRepository
{
    public EventRepository(GameDbContext context) : base(context)
    {
    }
}