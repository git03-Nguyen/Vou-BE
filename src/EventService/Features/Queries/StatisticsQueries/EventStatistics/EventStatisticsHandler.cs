using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace EventService.Features.Queries.StatisticsQueries.EventStatistics;

public class EventStatisticsHandler : IRequestHandler<EventStatisticsQuery, BaseResponse<EventStatisticsResponseDto>>
{
    private readonly ILogger<EventStatisticsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public EventStatisticsHandler(ILogger<EventStatisticsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<EventStatisticsResponseDto>> Handle(EventStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<EventStatisticsResponseDto>();
        const string methodName = $"{nameof(EventStatisticsHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var totalEvents = await _unitOfWork.Events.GetAll().CountAsync(x => !x.IsDeleted, cancellationToken);
            var totalPlayers = await _unitOfWork.Players.GetAll().CountAsync(cancellationToken);
            var totalVouchers = await _unitOfWork.Vouchers.GetAll().CountAsync(x => !x.IsDeleted, cancellationToken);
            var totalIssuedVouchers = await _unitOfWork.VoucherToPlayers.GetAll().CountAsync(x => !x.IsDeleted, cancellationToken);
            var totalRedeemedVouchers = await _unitOfWork.VoucherToPlayers.GetAll().CountAsync(x => !x.IsDeleted && x.UsedDate != null, cancellationToken);
            var totalAvailableVoucherStock = await _unitOfWork.Vouchers.GetAll()
                .Where(x => !x.IsDeleted && x.TotalQuantity.HasValue)
                .Select(x => new
                {
                    x.TotalQuantity,
                    Issued = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == x.Id)
                })
                .Select(x => Math.Max(x.TotalQuantity!.Value - x.Issued, 0))
                .SumAsync(cancellationToken);
            
            var eventStatistics = new EventStatisticsResponseDto
            {
                TotalPlayers = totalPlayers,
                TotalActiveEvents = totalEvents,
                TotalVouchers = totalVouchers,
                TotalIssuedVouchers = totalIssuedVouchers,
                TotalRedeemedVouchers = totalRedeemedVouchers,
                TotalAvailableVoucherStock = totalAvailableVoucherStock
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
