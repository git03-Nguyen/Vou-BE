using GameService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Common;
using Shared.Services.HttpContextAccessor;

namespace GameService.SignalRHubs.QuizGame;

[Authorize(Roles = Constants.PLAYER)]
public class QuizGameHub : Hub<IQuizClient>
{
    private readonly ILogger<QuizGameHub> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public QuizGameHub(ILogger<QuizGameHub> logger, ICustomHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(OnConnectedAsync)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(QuizGameHub)}.{nameof(OnDisconnectedAsync)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        await base.OnDisconnectedAsync(exception);
    }
}