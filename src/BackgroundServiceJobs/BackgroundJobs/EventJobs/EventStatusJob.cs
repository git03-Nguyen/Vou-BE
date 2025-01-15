using Microsoft.EntityFrameworkCore;
using PaymentService.Data.Models;
using PaymentService.Repositories;
using Shared.Enums;

namespace PaymentService.BackgroundJobs.EventJobs;

public class EventStatusJob
{
    private readonly ILogger<EventStatusJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public EventStatusJob(ILogger<EventStatusJob> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateEventStatuses()
    {
        var currentTime = DateTime.Now;
        var methodName = $"{nameof(EventStatusJob)}.{nameof(UpdateEventStatuses)} CurrentTime: {currentTime} =>";
        _logger.LogInformation(methodName);

        try
        {
            // Update events from Approved to InProgress
            var approvedEvents = await _unitOfWork.Events
                .Where(x => !x.IsDeleted && x.Status == EventStatus.Approved)
                .ToListAsync(CancellationToken.None);

            var updatedEvents = new List<Event>();
            foreach (var e in approvedEvents)
            {
                if (e.StartDate <= currentTime)
                {
                    e.Status = EventStatus.InProgress;
                    updatedEvents.Add(e);
                }
            }
            if (updatedEvents.Count != 0)
            {
                _unitOfWork.Events.UpdateRange(updatedEvents);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            }
        
            // Update events from InProgress to Finished
            updatedEvents.Clear();
            var inProgressEvents = await _unitOfWork.Events
                .Where(x => !x.IsDeleted && x.Status == EventStatus.InProgress)
                .ToListAsync(CancellationToken.None);
            foreach (var e in inProgressEvents)
            {
                if (e.EndDate <= currentTime)
                {
                    e.Status = EventStatus.Finished;
                    updatedEvents.Add(e);
                }
            }
            if (updatedEvents.Count != 0)
            {
                _unitOfWork.Events.UpdateRange(updatedEvents);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{methodName} Has error: {e.Message}");
        }
    }
}