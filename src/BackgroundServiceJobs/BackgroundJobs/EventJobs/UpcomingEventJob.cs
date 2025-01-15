using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentService.Data.Models;
using PaymentService.Options;
using PaymentService.Repositories;
using PaymentService.Services.EventPublishService;
using Shared.Contracts;
using Shared.Enums;

namespace PaymentService.BackgroundJobs.EventJobs;

public class UpcomingEventJob
{
    private readonly ILogger<EventStatusJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HangfireOptions _hangfireOptions;
    private readonly IEventPublishService _eventPublishService;
    public UpcomingEventJob(ILogger<EventStatusJob> logger, IUnitOfWork unitOfWork, IOptions<HangfireOptions> hangfireOptions, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _eventPublishService = eventPublishService;
        _hangfireOptions = hangfireOptions.Value;
    }
    
    public async Task NotifyUpcomingEvent(string eventId)
    {
        var methodName = $"{nameof(UpcomingEventJob)}.{nameof(NotifyUpcomingEvent)} EventId = {eventId} =>";
        _logger.LogInformation(methodName);

        try
        {
            // Check event exists or deleted
            var currentEvent = await _unitOfWork.Events
                .Where(e => !e.IsDeleted && e.Id == eventId)
                .AsNoTracking()
                .FirstOrDefaultAsync(CancellationToken.None);
            if (currentEvent == null)
            {
                _logger.LogInformation($"{methodName} Event not found");
                return;
            }
            
            // Get favorite events
            var notifications = await
            (
                from @event in _unitOfWork.Events.GetAll()
                join favEvent in _unitOfWork.FavoriteEvents.GetAll()
                    on @event.Id equals favEvent.EventId
                where !favEvent.HasNotified && favEvent.EventId == eventId
                select new Notification
                {
                    PlayerId = favEvent.PlayerId,
                    Title = $"Upcoming event: {@event.Name}",
                    Content = $"Big news!!! The event {@event.Name} will start at {@event.StartDate} and end at {@event.EndDate}"
                }
            )
            .AsNoTracking()
            .ToListAsync(CancellationToken.None);
            
            if (notifications.Count == 0)
            {
                _logger.LogInformation($"{methodName} No notification to send");
                return;
            }
            
            await _unitOfWork.Notifications.AddRangeAsync(notifications, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            
            // Publish notifications
            var batch = notifications.Select(n => new NotificationDto
            {
                Content = n.Content,
                Title = n.Title,
                PlayerId = n.PlayerId,
                Id = n.Id,
                CreatedDate = n.CreatedDate ?? DateTime.Now,
                IsRead = n.IsRead
            });
            await _eventPublishService.PublishNotificationsAsync(batch, CancellationToken.None);
            
            // Has notified
            var favEvents = await _unitOfWork.FavoriteEvents
                .Where(f => f.EventId == eventId)
                .ToListAsync(CancellationToken.None);
            favEvents.ForEach(f => f.HasNotified = true);
            _unitOfWork.FavoriteEvents.UpdateRange(favEvents);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"{methodName} Has error: {ex.Message}");
        }
    }
}