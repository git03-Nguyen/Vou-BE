using GameService.Data.Models;
using GameService.DTOs;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventHandler : IRequestHandler<GetTicketEventQuery, BaseResponse<PlayerShakeDto>>
{
    private readonly ILogger<GetTicketEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetTicketEventHandler(ILogger<GetTicketEventHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<PlayerShakeDto>> Handle(GetTicketEventQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetTicketEventHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<PlayerShakeDto>();

        try
        {
            var @event = await _unitOfWork.Events
                .Where(x => x.Id == request.EventId)
                            // && (x.Status == EventStatus.Approved || x.Status == EventStatus.InProgress))
                .Select(x => new { x.Id })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (@event is null)
            {
                response.ToBadRequestResponse("Event not found or not accepted");
                return response;
            }

            var player = await GetOrCreatePlayerShakeSessionAsync(userId, request.EventId, cancellationToken);

            // If player has already joined the event, check if the reset time has passed
            if (player.NextResetTicketsTime < DateTime.Now)
            {
                var newPlayer = new PlayerShakeSession();
                player.Tickets = newPlayer.Tickets;
                player.NextResetTicketsTime = newPlayer.NextResetTicketsTime;
                _unitOfWork.PlayerShakeSessions.Update(player);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var responseData = new PlayerShakeDto
            {
                Id = userId,
                Tickets = player.Tickets,
                Diamonds = player.Diamond
            };

            return response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }

    private async Task<PlayerShakeSession> GetOrCreatePlayerShakeSessionAsync(
        string userId,
        string eventId,
        CancellationToken cancellationToken)
    {
        var existingSession = await _unitOfWork.PlayerShakeSessions
            .Where(x => x.PlayerId == userId
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
                .Where(x => x.PlayerId == userId
                            && x.EventId == eventId
                            && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingSession is not null)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return existingSession;
            }

            var newSession = new PlayerShakeSession
            {
                EventId = eventId,
                PlayerId = userId
            };

            await _unitOfWork.PlayerShakeSessions.AddAsync(newSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return newSession;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            var existingAfterConflict = await _unitOfWork.PlayerShakeSessions
                .Where(x => x.PlayerId == userId
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