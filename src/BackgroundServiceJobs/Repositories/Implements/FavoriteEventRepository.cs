using PaymentService.Data.Contexts;
using PaymentService.Data.Models;
using PaymentService.Repositories.Interfaces;
using Shared.Repositories;

namespace PaymentService.Repositories.Implements;

public class FavoriteEventRepository : GenericRepository<EventDbContext, FavoriteEvent>, IFavoriteEventRepository
{
    public FavoriteEventRepository(EventDbContext context) : base(context)
    {
    }
}