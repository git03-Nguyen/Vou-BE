using BackgroundServiceJobs.Options;
using BackgroundServiceJobs.Repositories;
using BackgroundServiceJobs.Services.EventPublishService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackgroundServiceJobs.BackgroundJobs.QuizSessionJobs;

public class QuizSessionJob
{
    private readonly ILogger<QuizSessionJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HangfireOptions _hangfireOptions;
    private readonly IEventPublishService _eventPublishService;
    public QuizSessionJob(ILogger<QuizSessionJob> logger, IUnitOfWork unitOfWork, IOptions<HangfireOptions> hangfireOptions, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _eventPublishService = eventPublishService;
        _hangfireOptions = hangfireOptions.Value;
    }
    
    public async Task StartQuizSession(string quizSessionId)
    {
        var methodName = $"{nameof(QuizSessionJob)}.{nameof(StartQuizSession)} QuizSessionId = {quizSessionId} =>";
        _logger.LogInformation(methodName);

        try
        {
            // Check quiz session exists or deleted
            var currentQuizSession = await _unitOfWork.QuizSessions
                .Where(e => !e.IsDeleted && e.Id == quizSessionId)
                .AsNoTracking()
                .FirstOrDefaultAsync(CancellationToken.None);
            
            //...
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"{methodName} Has error: {ex.Message}");
        }
    }
}