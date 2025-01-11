using EventService.Data.Contexts;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories;

public class UnitOfWork : GenericUnitOfWork<EventDbContext>, IUnitOfWork
{
    public UnitOfWork(EventDbContext dbContext) : base(dbContext)
    {
    }

    public IVoucherRepository Vouchers { get; set; }
    public IVoucherInEventRepository VoucherInEvents { get; set; }
    public IVoucherToPlayerRepository VoucherToPlayers { get; set; }
    public ICounterPartRepository CounterParts { get; set; }
    public IEventRepository Events { get; set; }
}