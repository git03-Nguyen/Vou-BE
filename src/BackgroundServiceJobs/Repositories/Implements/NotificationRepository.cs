using PaymentService.Data.Contexts;
using PaymentService.Data.Models;
using PaymentService.Repositories.Interfaces;
using Shared.Repositories;

namespace PaymentService.Repositories.Implements;

public class NotificationRepository : GenericRepository<EventDbContext, Notification>, INotificationRepository
{
    public NotificationRepository(EventDbContext context) : base(context)
    {
    }
}