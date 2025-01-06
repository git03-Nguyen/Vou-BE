using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implementation;

public class UserRepository : GenericRepository<AuthDbContext, User>, IUserRepository
{
    public UserRepository(AuthDbContext context) : base(context)
    {
    }
}
