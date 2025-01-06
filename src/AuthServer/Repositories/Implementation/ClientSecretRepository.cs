using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implementation;

public class ClientSecretRepository :  GenericRepository<AuthDbContext, ClientSecret>, IClientSecretRepository
{
    public ClientSecretRepository(AuthDbContext context) : base(context)
    {
    }
}