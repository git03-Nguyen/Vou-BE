using BackgroundServiceJobs.Data.Contexts;
using BackgroundServiceJobs.Data.Models;
using BackgroundServiceJobs.Repositories.Interfaces;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories.Implements;

public class EventRepository : GenericRepository<EventDbContext, Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }
}