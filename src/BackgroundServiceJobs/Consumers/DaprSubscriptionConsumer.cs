using System.Text.Json;
using BackgroundServiceJobs.BackgroundJobs.EventJobs;
using BackgroundServiceJobs.BackgroundJobs.QuizSessionJobs;
using BackgroundServiceJobs.Options;
using BackgroundServiceJobs.Repositories;
using Dapr;
using Dapr.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Common;
using Shared.Contracts.EventMessages;

namespace BackgroundServiceJobs.Consumers;

[ApiController]
[Route("dapr/[controller]")]
public class DaprSubscriptionConsumer : ControllerBase
{
    private readonly ILogger<DaprSubscriptionConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HangfireOptions _hangfireOptions;
    public DaprSubscriptionConsumer(ILogger<DaprSubscriptionConsumer> logger, IUnitOfWork unitOfWork, IOptions<HangfireOptions> hangfireOptions)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _hangfireOptions = hangfireOptions.Value;
    }

    [HttpPost($"{nameof(EventCreatedNotifier)}_Route")]
    [Topic(Constants.PubSubName, nameof(EventCreatedNotifier), $"{nameof(EventCreatedNotifier)}_DeadLetter", false)]
    [BulkSubscribe(nameof(EventCreatedNotifier), 500, 2000)]
    public async Task<IActionResult> HandleEventUpdatedAsync
    (
        [FromBody] BulkSubscribeMessage<BulkMessageModel<EventCreatedNotifier>> bulkMessage,
        CancellationToken cancellationToken
    )
    {
        foreach (var entry in bulkMessage.Entries)
        {
            var message = entry.Event.Data;
            var methodName = $"{nameof(DaprSubscriptionConsumer)}.{nameof(HandleEventUpdatedAsync)} Source = {entry.Event.Source}, Message = {JsonSerializer.Serialize(message)} =>";
            _logger.LogInformation(methodName);

            try
            {
                if (message.StartDate is null)
                {
                    _logger.LogCritical($"{methodName} StartDate is null");
                    return BadRequest();
                }
                
                // Schedule jobs to update event status
                BackgroundJob.Schedule<EventStatusJob>(x => x.UpdateEventToInProgress(message.EventId), message.StartDate.Value);
                BackgroundJob.Schedule<EventStatusJob>(x => x.UpdateEventToFinished(message.EventId), message.EndDate.Value);
                
                // Schedule jobs to notify player like event
                var timeToNotifyPlayer = message.StartDate.Value.AddSeconds(-_hangfireOptions.SecondsBeforeToNotifyEvent);
                BackgroundJob.Schedule<UpcomingEventJob>(x => x.NotifyUpcomingEvent(message.EventId), timeToNotifyPlayer);
                
                // Schedule jobs to start quizsession
                var quizSessions = await _unitOfWork.QuizSessions
                    .Where(x => x.EventId == message.EventId)
                    .Select(x => new
                    {
                        x.Id,
                        x.StartTime
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                foreach (var quizSession in quizSessions)
                {
                    BackgroundJob.Schedule<QuizSessionJob>(x => x.StartQuizSession(quizSession.Id), quizSession.StartTime);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{methodName} Has error: {e.Message}");
                return BadRequest();
            }
        }
        
        return Ok();
    }
}