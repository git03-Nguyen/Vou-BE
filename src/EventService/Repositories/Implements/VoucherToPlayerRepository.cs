using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class VoucherToPlayerRepository : GenericRepository<EventDbContext, VoucherToPlayer>, IVoucherToPlayerRepository
{
    public VoucherToPlayerRepository(EventDbContext context) : base(context)
    {
    }
}