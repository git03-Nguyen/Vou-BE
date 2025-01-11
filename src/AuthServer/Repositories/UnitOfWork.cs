using AuthServer.Data.Contexts;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories;

public class UnitOfWork : GenericUnitOfWork<AuthDbContext>, IUnitOfWork
{
    public UnitOfWork
    (
        AuthDbContext dbContext,
        IPlayerRepository playerRepository,
        ICounterPartRepository counterPartRepository
    ) : base(dbContext)
    {
        Players = playerRepository;
        CounterParts = counterPartRepository;
    }

    public IPlayerRepository Players { get; set; }
    public ICounterPartRepository CounterParts { get; set; }
}