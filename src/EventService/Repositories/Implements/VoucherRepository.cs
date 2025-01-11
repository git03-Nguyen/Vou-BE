using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class VoucherRepository : GenericRepository<EventDbContext, Voucher>, IVoucherRepository
{
    public VoucherRepository(EventDbContext context) : base(context)
    {
    }
}