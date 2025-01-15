using BackgroundServiceJobs.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace BackgroundServiceJobs.BackgroundJobs.EventJobs;

public class EventStatusJob
{
    private readonly ILogger<EventStatusJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public EventStatusJob(ILogger<EventStatusJob> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateEventToInProgress(string eventId)
    {
        var methodName = $"{nameof(EventStatusJob)}.{nameof(UpdateEventToInProgress)} EventId = {eventId} =>";
        _logger.LogInformation(methodName);

        try
        {
            var @event = await _unitOfWork.Events
                .Where(x =>
                    !x.IsDeleted
                    && x.Id == eventId
                    && (x.Status == EventStatus.Approved || x.Status == EventStatus.InProgress))
                .FirstOrDefaultAsync(CancellationToken.None);
            
            if (@event is null)
            {
                _logger.LogInformation($"{methodName} Event not found or status is not Approved");
                return;
            }
            
            // Update events from Approved to InProgress
            if (@event.Status == EventStatus.Approved)
            {
                @event.Status = EventStatus.InProgress;
                _unitOfWork.Events.Update(@event);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{methodName} Has error: {e.Message}");
        }
    }
    
    public async Task UpdateEventToFinished(string eventId)
    {
        var methodName = $"{nameof(EventStatusJob)}.{nameof(UpdateEventToFinished)} EventId = {eventId} =>";
        _logger.LogInformation(methodName);

        try
        {
            var @event = await _unitOfWork.Events
                .Where(x =>
                    !x.IsDeleted
                    && x.Id == eventId
                    && x.Status == EventStatus.InProgress)
                .FirstOrDefaultAsync(CancellationToken.None);
            
            if (@event is null)
            {
                _logger.LogInformation($"{methodName} Event not found or status is not InProgress");
                return;
            }
            
            // Update events from InProgress to Finished
            if (@event.Status == EventStatus.InProgress)
            {
                @event.Status = EventStatus.Finished;
                _unitOfWork.Events.Update(@event);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{methodName} Has error: {e.Message}");
        }
    }
}