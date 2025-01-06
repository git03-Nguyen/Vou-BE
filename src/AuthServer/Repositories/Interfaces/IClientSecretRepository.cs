using AuthServer.Data.Entities;
using Shared.Repositories;

namespace AuthServer.Repositories.Interfaces;

public interface IClientSecretRepository : IGenericRepository<ClientSecret>
{
}