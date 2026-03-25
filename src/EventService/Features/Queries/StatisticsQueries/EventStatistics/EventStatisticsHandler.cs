using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;

namespace EventService.Features.Queries.StatisticsQueries.EventStatistics;

public class EventStatisticsHandler : IRequestHandler<EventStatisticsQuery, BaseResponse<AdminEventStatisticsResponseDto>>
{
    private readonly ILogger<EventStatisticsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public EventStatisticsHandler(ILogger<EventStatisticsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<AdminEventStatisticsResponseDto>> Handle(EventStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<AdminEventStatisticsResponseDto>();
        const string methodName = $"{nameof(EventStatisticsHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var eventsQuery = _unitOfWork.Events.GetAll().Where(x => !x.IsDeleted);
            var vouchersQuery = _unitOfWork.Vouchers.GetAll().Where(x => !x.IsDeleted);
            var voucherToPlayersQuery = _unitOfWork.VoucherToPlayers.GetAll().Where(x => !x.IsDeleted);

            var totalEvents = await eventsQuery.CountAsync(cancellationToken);
            var totalPendingEvents = await eventsQuery.CountAsync(x => x.Status == EventStatus.Pending, cancellationToken);
            var totalApprovedEvents = await eventsQuery.CountAsync(x => x.Status == EventStatus.Approved, cancellationToken);
            var totalInProgressEvents = await eventsQuery.CountAsync(x => x.Status == EventStatus.InProgress, cancellationToken);
            var totalFinishedEvents = await eventsQuery.CountAsync(x => x.Status == EventStatus.Finished, cancellationToken);
            var totalCanceledEvents = await eventsQuery.CountAsync(x => x.Status == EventStatus.Canceled, cancellationToken);
            var totalCounterParts = await _unitOfWork.CounterParts.GetAll().CountAsync(cancellationToken);
            var totalPlayers = await _unitOfWork.Players.GetAll().CountAsync(cancellationToken);
            var totalVouchers = await vouchersQuery.CountAsync(cancellationToken);
            var totalIssuedVouchers = await voucherToPlayersQuery.CountAsync(cancellationToken);
            var totalRedeemedVouchers = await voucherToPlayersQuery.CountAsync(x => x.UsedDate != null, cancellationToken);
            var totalEventsWithShakeGame = await eventsQuery.CountAsync(x => x.ShakeVoucherId != null, cancellationToken);
            var totalQuizSessions = await _unitOfWork.QuizSessions.GetAll().CountAsync(x => !x.IsDeleted, cancellationToken);
            var totalAvailableVoucherStock = await vouchersQuery
                .Where(x => x.TotalQuantity.HasValue)
                .Select(x => new
                {
                    x.TotalQuantity,
                    Issued = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == x.Id)
                })
                .Select(x => Math.Max(x.TotalQuantity!.Value - x.Issued, 0))
                .SumAsync(cancellationToken);

            var eventStatistics = new AdminEventStatisticsResponseDto
            {
                TotalEvents = totalEvents,
                TotalPendingEvents = totalPendingEvents,
                TotalApprovedEvents = totalApprovedEvents,
                TotalInProgressEvents = totalInProgressEvents,
                TotalFinishedEvents = totalFinishedEvents,
                TotalCanceledEvents = totalCanceledEvents,
                TotalCounterParts = totalCounterParts,
                TotalPlayers = totalPlayers,
                TotalVouchers = totalVouchers,
                TotalIssuedVouchers = totalIssuedVouchers,
                TotalRedeemedVouchers = totalRedeemedVouchers,
                TotalAvailableVoucherStock = totalAvailableVoucherStock,
                TotalEventsWithShakeGame = totalEventsWithShakeGame,
                TotalQuizSessions = totalQuizSessions
            };

            response.ToSuccessResponse(eventStatistics);
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
