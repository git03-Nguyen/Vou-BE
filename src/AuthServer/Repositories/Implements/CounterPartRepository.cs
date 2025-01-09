using AuthServer.Data.Contexts;
using AuthServer.Data.Models;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implements;

public class CounterPartRepository : GenericRepository<AuthDbContext, CounterPart>, ICounterPartRepository
{
    public CounterPartRepository(AuthDbContext context) : base(context)
    {
    }
}