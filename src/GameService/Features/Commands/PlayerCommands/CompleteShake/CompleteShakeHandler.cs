using GameService.DTOs;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Commands.PlayerCommands.CompleteShake;

public class CompleteShakeHandler : IRequestHandler<CompleteShakeCommand, BaseResponse<ShakeResultDto>>
{
    private readonly ILogger<CompleteShakeHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public CompleteShakeHandler(ILogger<CompleteShakeHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<ShakeResultDto>> Handle(CompleteShakeCommand request, CancellationToken cancellationToken)
    {
        var methodName = $"{nameof(CompleteShakeHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<ShakeResultDto>();

        try
        {
            var userId = _contextAccessor.GetCurrentUserId();
            var shakeSession = await _unitOfWork.PlayerShakeSessions
                .Where(x =>
                    x.PlayerId == userId 
                    && !x.IsDeleted
                    && x.EventId == request.EventId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (shakeSession is null)
            {
                response.ToBadRequestResponse("Shake session not found");
                return response;
            }

            if (shakeSession.Tickets <= 0)
            {
                response.ToBadRequestResponse("No ticket left");
                return response;
            }
            
            var @event = await _unitOfWork.Events
                .Where(x => 
                    x.Id == request.EventId
                    && x.Status == EventStatus.InProgress)
                .Select(x => new
                {
                    x.ShakeAverageDiamond,
                    x.ShakeWinRate
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            if (@event is null)
            {
                response.ToNotFoundResponse("Event not found");
                return response;
            }
            
            shakeSession.Tickets--;
            var diamondsReceived = GetRandomDiamondsReceived(@event.ShakeAverageDiamond ?? 0, @event.ShakeWinRate ?? 0);
            shakeSession.Diamond += diamondsReceived;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var shakeResult = new ShakeResultDto
            {
                Tickets = shakeSession.Tickets,
                Diamonds = shakeSession.Diamond
            };
            response.ToSuccessResponse(shakeResult);
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
            return response;
        }
    }
    
    private static int GetRandomDiamondsReceived(int averageDiamond, int winRate)
    {
        var random = new Random();
        var isLucky = random.Next(0, 100) < winRate;
        if (!isLucky)
        {
            return 0;
        }

        var minDiamond = Math.Min((int)(averageDiamond * 0.8), 0); // 80% of average diamond
        var maxDiamond = (int)(averageDiamond * 1.2); // 120% of average diamond
        return random.Next(minDiamond, maxDiamond);
    }
}