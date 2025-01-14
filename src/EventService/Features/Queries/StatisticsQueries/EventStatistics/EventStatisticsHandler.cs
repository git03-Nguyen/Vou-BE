using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Enums;
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
            var events =await (
                from @event in _unitOfWork.Events.GetAll()
                where !@event.IsDeleted && @event.Status==EventStatus.Approved|| @event.Status==EventStatus.InProgress
                    select @event
            )
                .ToListAsync(cancellationToken);
            
            var counterParts=await (
                from @counterPart in _unitOfWork.CounterParts.GetAll()
                where !@counterPart.IsBlocked==false
                    select @counterPart
            )
                .ToListAsync(cancellationToken);
            
            var players=await (
                from player in _unitOfWork.Players.GetAll()
                    select player
            )
                .ToListAsync(cancellationToken);
            
            var vouchers=await (
                from voucher in _unitOfWork.Vouchers.GetAll()
                    select voucher
            )
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            var eventStatistics = new EventStatisticsResponseDto
            {
                TotalPlayers = players.Count,
                TotalActiveEvents = events.Count,
                TotalVouchers = vouchers.Count,
                TotalCounterParts = counterParts.Count
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