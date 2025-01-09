using EventService.Data.Contexts;
using Shared.Repositories;

namespace EventService.Repositories;

public class UnitOfWork : GenericUnitOfWork<EventDbContext>, IUnitOfWork
{
    public UnitOfWork(EventDbContext dbContext) : base(dbContext)
    {
    }
}