using EventService.Data.Contexts;
using EventService.Data.Models;
using EventService.Repositories.Interfaces;
using Shared.Repositories;

namespace EventService.Repositories.Implements;

public class NotificationRepository : GenericRepository<EventDbContext, Notification>, INotificationRepository
{
    public NotificationRepository(EventDbContext context) : base(context)
    {
    }
}