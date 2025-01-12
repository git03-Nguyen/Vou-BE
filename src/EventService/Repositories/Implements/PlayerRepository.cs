using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Data.Models.SyncModels;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class PlayerRepository : GenericRepository<EventDbContext, Player>, IPlayerRepository
{
    public PlayerRepository(EventDbContext context) : base(context)
    {
    }
}