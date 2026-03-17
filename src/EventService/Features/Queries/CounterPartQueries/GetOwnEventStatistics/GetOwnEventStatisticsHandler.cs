using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEventStatistics;

public class GetOwnEventStatisticsHandler : IRequestHandler<GetOwnEventStatisticsQuery, BaseResponse<CounterPartStatisticsEventsResponseDto>>
{
    private readonly ILogger<GetOwnEventStatisticsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetOwnEventStatisticsHandler(ILogger<GetOwnEventStatisticsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<CounterPartStatisticsEventsResponseDto>> Handle(GetOwnEventStatisticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnEventStatisticsHandler)}.{nameof(Handle)} UserId: {userId}, EventId: {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<CounterPartStatisticsEventsResponseDto>();

        try
        {
            var eventsQuery = _unitOfWork.Events.GetAll()
                .Where(x => !x.IsDeleted
                            && x.CounterPartId == userId
                            && (x.Status == EventStatus.InProgress || x.Status == EventStatus.Approved));

            var eventIds = await eventsQuery.Select(x => x.Id).ToListAsync(cancellationToken);
            var voucherIds = await _unitOfWork.Vouchers.GetAll()
                .Where(x => !x.IsDeleted && x.CounterPartId == userId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var totalEvents = eventIds.Count;
            var totalVouchers = voucherIds.Count;
            var totalVouchersValue = await _unitOfWork.Vouchers.GetAll()
                .Where(x => !x.IsDeleted && x.CounterPartId == userId)
                .SumAsync(x => x.Value, cancellationToken);
            var totalQuizSessions = await _unitOfWork.QuizSessions.GetAll()
                .CountAsync(x => !x.IsDeleted && eventIds.Contains(x.EventId), cancellationToken);
            var totalShakeGame = await eventsQuery.CountAsync(x => x.ShakeVoucherId != null, cancellationToken);
            var totalIssuedVouchers = await _unitOfWork.VoucherToPlayers.GetAll()
                .CountAsync(x => !x.IsDeleted && voucherIds.Contains(x.VoucherId), cancellationToken);
            var totalRedeemedVouchers = await _unitOfWork.VoucherToPlayers.GetAll()
                .CountAsync(x => !x.IsDeleted && voucherIds.Contains(x.VoucherId) && x.UsedDate != null, cancellationToken);
            var totalAvailableVoucherStock = await _unitOfWork.Vouchers.GetAll()
                .Where(x => !x.IsDeleted && x.CounterPartId == userId && x.TotalQuantity.HasValue)
                .Select(x => new
                {
                    x.TotalQuantity,
                    Issued = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == x.Id)
                })
                .Select(x => Math.Max(x.TotalQuantity!.Value - x.Issued, 0))
                .SumAsync(cancellationToken);

            var eventStatistics = new CounterPartStatisticsEventsResponseDto
            {
                TotalEvents = totalEvents,
                TotalVouchers = totalVouchers,
                TotalVouchersValue = totalVouchersValue,
                TotalQuizSessions = totalQuizSessions,
                TotalShakeGame = totalShakeGame,
                TotalIssuedVouchers = totalIssuedVouchers,
                TotalRedeemedVouchers = totalRedeemedVouchers,
                TotalAvailableVoucherStock = totalAvailableVoucherStock
            };

            response.ToSuccessResponse(eventStatistics);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }
        return response;
    }
}
