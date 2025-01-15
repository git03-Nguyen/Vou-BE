using BackgroundServiceJobs.Data.Contexts;
using BackgroundServiceJobs.Data.Models;
using BackgroundServiceJobs.Repositories.Interfaces;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories.Implements;

public class NotificationRepository : GenericRepository<EventDbContext, Notification>, INotificationRepository
{
    public NotificationRepository(EventDbContext context) : base(context)
    {
    }
}