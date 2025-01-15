using PaymentService.Data.Contexts;
using PaymentService.Data.Models;
using PaymentService.Repositories.Interfaces;
using Shared.Repositories;

namespace PaymentService.Repositories.Implements;

public class EventRepository : GenericRepository<EventDbContext, Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }
}