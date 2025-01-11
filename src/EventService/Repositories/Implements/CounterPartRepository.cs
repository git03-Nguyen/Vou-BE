using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Data.Models.SyncModels;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class CounterPartRepository : GenericRepository<EventDbContext, CounterPart>, ICounterPartRepository
{
    public CounterPartRepository(EventDbContext context) : base(context)
    {
    }
}