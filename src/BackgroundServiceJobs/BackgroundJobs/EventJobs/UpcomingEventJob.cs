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
    
    public async Task NotifyUpcomingEvents()
    {
        var currentTime = DateTime.Now;
        var methodName = $"{nameof(UpcomingEventJob)}.{nameof(NotifyUpcomingEvents)} CurrentTime: {currentTime} =>";
        _logger.LogInformation(methodName);

        try
        {
            // Get favorite events
            var events = await
            (
                from @event in _unitOfWork.Events.GetAll()
                join favEvent in _unitOfWork.FavoriteEvents.GetAll()
                    on @event.Id equals favEvent.EventId
                where !@event.IsDeleted
                      && @event.Status == EventStatus.Approved
                      && currentTime >= @event.StartDate.AddMinutes(-_hangfireOptions.MinutesBeforeToNotify)
                      && !favEvent.HasNotified
                select new
                {
                    PlayerId = favEvent.PlayerId,
                    EventId = @event.Id,
                    Name = @event.Name,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    ImageUrl = @event.ImageUrl
                }
            )
            .AsNoTracking()
            .ToListAsync(CancellationToken.None);

            var notifications = events.Select(e => new Notification
            {
                PlayerId = e.PlayerId,
                Title = $"Upcoming event: {e.Name}",
                Content = $"Big news!!! The event {e.Name} will start at {e.StartDate} and end at {e.EndDate}"
            }).ToList();

            if (notifications.Count == 0)
            {
                _logger.LogInformation($"{methodName} No upcoming events");
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
                CreatedDate = n.CreatedDate ?? currentTime,
                IsRead = n.IsRead
            });
            await _eventPublishService.PublishNotificationsAsync(batch, CancellationToken.None);
            
            // Has notified
            var favEventIds = events.Select(e => e.EventId).ToList();
            var favEvents = await _unitOfWork.FavoriteEvents
                .Where(f => favEventIds.Contains(f.EventId))
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