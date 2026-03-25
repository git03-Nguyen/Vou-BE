using GameService.Data.Models;
using GameService.Helpers;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Queries.PlayerQueries.GetEventPlayerState;

public class GetEventPlayerStateHandler : IRequestHandler<GetEventPlayerStateQuery, BaseResponse<EventPlayerStateDto>>
{
    private readonly ILogger<GetEventPlayerStateHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;

    public GetEventPlayerStateHandler(
        ILogger<GetEventPlayerStateHandler> logger,
        IUnitOfWork unitOfWork,
        ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<EventPlayerStateDto>> Handle(GetEventPlayerStateQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetEventPlayerStateHandler)}.{nameof(Handle)} UserId = {userId}, EventId = {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<EventPlayerStateDto>();

        try
        {
            var eventExists = await _unitOfWork.Events
                .Where(x => x.Id == request.EventId && (x.Status == EventStatus.Approved || x.Status == EventStatus.InProgress))
                .Select(x => x.Id)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (!eventExists)
            {
                response.ToNotFoundResponse("Event not found");
                return response;
            }

            var playerShakeSession = await GetOrCreatePlayerShakeSessionAsync(userId, request.EventId, cancellationToken);
            playerShakeSession = await ResetTicketsIfNeededAsync(playerShakeSession, cancellationToken);

            var quizHistory = await (
                from playerQuizSession in _unitOfWork.PlayerQuizSessions.GetAll()
                join quizSession in _unitOfWork.QuizSessions.GetAll()
                    on playerQuizSession.QuizSessionId equals quizSession.Id
                join quizSet in _unitOfWork.QuizSets.GetAll()
                    on quizSession.QuizSetId equals quizSet.Id into quizSetJoin
                from quizSet in quizSetJoin.DefaultIfEmpty()
                where !playerQuizSession.IsDeleted
                      && playerQuizSession.PlayerId == userId
                      && quizSession.EventId == request.EventId
                orderby playerQuizSession.CreatedDate descending
                select new PlayerQuizHistoryItemDto
                {
                    QuizSessionId = playerQuizSession.QuizSessionId,
                    QuizSetId = quizSession.QuizSetId,
                    QuizSetTitle = quizSet != null ? quizSet.Title : null,
                    Score = playerQuizSession.Score,
                    IsWin = playerQuizSession.IsWin,
                    PlayedAt = playerQuizSession.CreatedDate
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new EventPlayerStateDto
            {
                EventId = request.EventId,
                PlayerId = userId,
                Tickets = playerShakeSession.Tickets,
                Diamonds = playerShakeSession.Diamond,
                LastShareTime = playerShakeSession.LastShareTime,
                NextResetTicketsTime = playerShakeSession.NextResetTicketsTime,
                QuizSessionsPlayed = quizHistory.Count,
                QuizWins = quizHistory.Count(x => x.IsWin),
                HighestQuizScore = quizHistory.Count == 0 ? 0 : quizHistory.Max(x => x.Score),
                TotalQuizScore = quizHistory.Sum(x => x.Score),
                LastPlayedAt = quizHistory.FirstOrDefault()?.PlayedAt,
                RecentQuizHistory = quizHistory.Take(10).ToList()
            };

            response.ToSuccessResponse(responseData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }

    private async Task<PlayerShakeSession> GetOrCreatePlayerShakeSessionAsync(string userId, string eventId, CancellationToken cancellationToken)
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

    private async Task<PlayerShakeSession> ResetTicketsIfNeededAsync(PlayerShakeSession player, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        if (player.NextResetTicketsTime is null || player.NextResetTicketsTime >= now)
        {
            return player;
        }

        var nextResetTicketsTime = TimeHelpers.GetNextMonday();
        var affectedRows = await _unitOfWork.PlayerShakeSessions
            .Where(x => x.Id == player.Id
                        && !x.IsDeleted
                        && x.NextResetTicketsTime < now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Tickets, _ => 5)
                .SetProperty(x => x.NextResetTicketsTime, _ => nextResetTicketsTime)
                .SetProperty(x => x.ModifiedDate, _ => now), cancellationToken);

        if (affectedRows == 0)
        {
            return await _unitOfWork.PlayerShakeSessions
                .Where(x => x.Id == player.Id && !x.IsDeleted)
                .AsNoTracking()
                .FirstAsync(cancellationToken);
        }

        player.Tickets = 5;
        player.NextResetTicketsTime = nextResetTicketsTime;
        player.ModifiedDate = now;
        return player;
    }
}
