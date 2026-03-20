using System.Text.Json;
using GameService.Data.Models;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        var methodName = $"{nameof(SendTicketHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse();

        try
        {
            var friendId = await _unitOfWork.Players
                .Where(x =>
                    x.Email == request.UserNameOrEmail
                    || x.UserName == request.UserNameOrEmail)
                .Select(x => x.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (friendId is null)
            {
                response.ToNotFoundResponse("Player not found");
                return response;
            }

            if (friendId == userId)
            {
                response.ToBadRequestResponse("Cannot send ticket to yourself");
                return response;
            }

            var now = DateTime.UtcNow;
            var nextResetTicketsTime = new PlayerShakeSession().NextResetTicketsTime;

            var friendShakeSession = await GetOrCreateFriendShakeSessionAsync(friendId, request.EventId, cancellationToken);

            await _unitOfWork.PlayerShakeSessions
                .Where(x => x.Id == friendShakeSession.Id
                            && !x.IsDeleted
                            && x.NextResetTicketsTime < now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.NextResetTicketsTime, _ => nextResetTicketsTime)
                    .SetProperty(x => x.Tickets, _ => 5)
                    .SetProperty(x => x.ModifiedDate, _ => now), cancellationToken);

            var affectedRows = await _unitOfWork.PlayerShakeSessions
                .Where(x => x.Id == friendShakeSession.Id
                            && !x.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Tickets, x => x.Tickets + 1)
                    .SetProperty(x => x.ModifiedDate, _ => now), cancellationToken);

            if (affectedRows == 0)
            {
                response.ToNotFoundResponse("Player shake session not found");
                return response;
            }

            response.ToSuccessResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }

    private async Task<PlayerShakeSession> GetOrCreateFriendShakeSessionAsync(
        string friendId,
        string eventId,
        CancellationToken cancellationToken)
    {
        var existingSession = await _unitOfWork.PlayerShakeSessions
            .Where(x => x.PlayerId == friendId
                        && x.EventId == eventId
                        && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSession is not null)
        {
            return existingSession;
        }

        await using var transaction = await _unitOfWork.OpenTransactionAsync(cancellationToken);

        try
        {
            existingSession = await _unitOfWork.PlayerShakeSessions
                .Where(x => x.PlayerId == friendId
                            && x.EventId == eventId
                            && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingSession is not null)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return existingSession;
            }

            var newShakeSession = new PlayerShakeSession
            {
                PlayerId = friendId,
                EventId = eventId,
                Tickets = 6
            };

            await _unitOfWork.PlayerShakeSessions.AddAsync(newShakeSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return newShakeSession;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            var existingAfterConflict = await _unitOfWork.PlayerShakeSessions
                .Where(x => x.PlayerId == friendId
                            && x.EventId == eventId
                            && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingAfterConflict is not null)
            {
                return existingAfterConflict;
            }

            throw;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
