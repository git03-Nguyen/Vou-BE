using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IEventRepository Events { get; set; }
    IVoucherRepository Vouchers { get; set; }
    IVoucherInEventRepository VoucherInEvents { get; set; }
    IVoucherToPlayerRepository VoucherToPlayers { get; set; }
}