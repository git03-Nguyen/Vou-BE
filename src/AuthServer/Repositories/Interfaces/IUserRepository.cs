using AuthServer.Data.Entities;
using Shared.Repositories;

namespace AuthServer.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
}