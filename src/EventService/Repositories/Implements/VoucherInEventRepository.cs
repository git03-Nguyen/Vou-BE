using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class VoucherInEventRepository : GenericRepository<EventDbContext, VoucherInEvent>, IVoucherInEventRepository
{
    public VoucherInEventRepository(EventDbContext context) : base(context)
    {
    }
}