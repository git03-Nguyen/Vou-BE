using GameService.Data.Models;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Commands.PlayerCommands.SendTicketToFriend;

public class SendTicketHandler : IRequestHandler<SendTicketCommand, BaseResponse>
{
    private readonly ILogger<SendTicketHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public SendTicketHandler(ILogger<SendTicketHandler> logger, ICustomHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse> Handle(SendTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(SendTicketHandler)}.{nameof(Handle)} UserId: {userId}, PlayerId {request.PlayerId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse();

        try
        {
            var friendId = await _unitOfWork.Players
                .Where(x => x.Id == request.PlayerId)
                .Select(x => x.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            if (friendId is null)
            {
                response.ToNotFoundResponse("Player not found");
                return response;
            }
            
            var friendShakeSession = await _unitOfWork.PlayerShakeSessions
                .Where(x => x.PlayerId == friendId && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            await _unitOfWork.OpenTransactionAsync(cancellationToken);
            if (friendShakeSession is null)
            {
                var eventId = request.EventId;
                var newShakeSession = new PlayerShakeSession
                {
                    PlayerId = friendId,
                    EventId = eventId
                };
                newShakeSession.Tickets++;
                await _unitOfWork.PlayerShakeSessions.AddAsync(newShakeSession, cancellationToken);
            }
            else
            {
                friendShakeSession.Tickets++;
                _unitOfWork.PlayerShakeSessions.Update(friendShakeSession);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            response.ToSuccessResponse();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, methodName);
            response.ToInternalErrorResponse();
        }

        return response;
    }
}