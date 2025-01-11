using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class EventRepository : GenericRepository<EventDbContext, Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }
}