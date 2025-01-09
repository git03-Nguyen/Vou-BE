using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IPlayerRepository Players { get; set; }
    ICounterPartRepository CounterParts { get; set; }
}