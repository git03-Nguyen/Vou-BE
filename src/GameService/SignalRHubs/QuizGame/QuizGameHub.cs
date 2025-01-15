using GameService.DTOs.RealtimeDtos;
using GameService.Extensions;
using GameService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Common;
using Shared.Services.CachingServices.DistributedCache;
using Shared.Services.HttpContextAccessor;

namespace GameService.SignalRHubs.QuizGame;

[Authorize(Roles = Constants.PLAYER)]
public class QuizGameHub : Hub<IQuizClient>
{
    private readonly ILogger<QuizGameHub> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IDaprStateStoreService _stateStore;
    public QuizGameHub(ILogger<QuizGameHub> logger, ICustomHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork, IDaprStateStoreService stateStore)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
        _stateStore = stateStore;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(OnConnectedAsync)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, $"{nameof(QuizGameHub)}.{nameof(OnDisconnectedAsync)} Exception =>");
        }
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(OnDisconnectedAsync)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task JoinGame(string quizSessionId)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(JoinGame)} UserId: {userId}, QuizSessionId: {quizSessionId} =>";
        _logger.LogInformation(methodName);
        
        // Increase the number of players in the quiz session
        var waitingPlayers = await _stateStore.GetAsync<List<WaitingPlayerDto>>(quizSessionId, Context.ConnectionAborted) ?? [];
        var isPlayerAlreadyWaiting = waitingPlayers.Any(x => x.Id == userId);
        if (isPlayerAlreadyWaiting)
        {
            Context.Abort();
        }
    
        waitingPlayers.Add(new WaitingPlayerDto
        {
            Id = userId,
            ConnectionId = Context.ConnectionId,
            UserName = _contextAccessor.GetCurrentUserName().ToMaskedUserName()
        });
        await _stateStore.SetAsync(Constants.WaitingPlayers, waitingPlayers, Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, quizSessionId, Context.ConnectionAborted);
        
        // Notify the client about the waiting players
        await Clients.Group(quizSessionId).WaitingPlayers(waitingPlayers);
    }
    
    public async Task LeaveGame(string quizSessionId)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(LeaveGame)} UserId: {userId}, QuizSessionId: {quizSessionId} =>";
        _logger.LogInformation(methodName);
        
        // Decrease the number of players in the quiz session
        var waitingPlayers = await _stateStore.GetAsync<List<WaitingPlayerDto>>(quizSessionId, Context.ConnectionAborted) ?? [];
        waitingPlayers.RemoveAll(x => x.Id == userId);
        await _stateStore.SetAsync(Constants.WaitingPlayers, waitingPlayers, Context.ConnectionAborted);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, quizSessionId, Context.ConnectionAborted);
        
        // Notify the client about the waiting players
        await Clients.Group(quizSessionId).WaitingPlayers(waitingPlayers);
    }
    
    public async Task SendAnswer(string quizSessionId, string questionId, int answerIndex)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(SendAnswer)} UserId: {userId}, QuizSessionId: {quizSessionId}, QuestionId: {questionId}, AnswerIndex: {answerIndex} =>";
        _logger.LogInformation(methodName);
        
        // Calculate the score based on the answer index and time
        var score = await CalculateScoreAsync(quizSessionId, answerIndex, Context.ConnectionAborted);
        
        // Notify the client about the result
        await Clients.Caller.Result(questionId, score);
    }

    #region Private methods

    private string WaitingPlayersKey(string quizSessionId) => $"{Constants.WaitingPlayers}:{quizSessionId}";
    private string CurrentQuestionKey(string quizSessionId) => $"{Constants.CurrentQuestion}:{quizSessionId}";
    
    private async Task<int> CalculateScoreAsync(string quizSessionId, int answerIndex, CancellationToken cancellationToken)
    {
        var currentQuestion = await _stateStore.GetAsync<CurrentQuestionDto>(CurrentQuestionKey(quizSessionId), cancellationToken);
        if (answerIndex != currentQuestion?.AnswerIndex)
        {
            return 0;
        }
     
        // Calculate the score based on the answer index and time
        var now = DateTime.Now;
        var timeDiff = (int) (now - currentQuestion.QuestionTime).TotalSeconds;
        return timeDiff switch
        {
            <= 10 => 100 - timeDiff,
            <= 20 => 80 - timeDiff,
            <= 30 => 60 - timeDiff,
            <= 40 => 40 - timeDiff,
            _ => 0
        };
    }

    #endregion
}