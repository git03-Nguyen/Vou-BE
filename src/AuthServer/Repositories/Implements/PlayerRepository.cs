using AuthServer.Data.Contexts;
using AuthServer.Data.Models;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implements;

public class PlayerRepository : GenericRepository<AuthDbContext, Player>, IPlayerRepository
{
    public PlayerRepository(AuthDbContext context) : base(context)
    {
    }
}