using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implementation;

public class ClientGrantTypeRepository :  GenericRepository<AuthDbContext, ClientGrantType>, IClientGrantTypeRepository
{
    public ClientGrantTypeRepository(AuthDbContext context) : base(context)
    {
    }
}