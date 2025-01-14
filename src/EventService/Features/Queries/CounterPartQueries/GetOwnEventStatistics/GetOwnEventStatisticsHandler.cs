using EventService.Data.Models;
using EventService.DTOs;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvent;
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
        var methodName = $"{nameof(GetOwnEventHandler)}.{nameof(Handle)} UserId: {userId}, EventId: {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<CounterPartStatisticsEventsResponseDto>();

        try
        { 
            var eventStatistics = await (
                    from @event in _unitOfWork.Events.GetAll()
                    join counterpart in _unitOfWork.CounterParts.GetAll()
                        on @event.CounterPartId equals counterpart.Id
                    where !@event.IsDeleted &&
                          @event.CounterPartId == userId &&
                          (@event.Status == EventStatus.InProgress || @event.Status == EventStatus.Approved)
                    join voucher in _unitOfWork.Vouchers.GetAll()
                        on @event.ShakeVoucherId equals voucher.Id into eventVouchers
                    from eventVoucher in eventVouchers.DefaultIfEmpty()
                    join quizSession in _unitOfWork.QuizSessions.GetAll()
                        on @event.Id equals quizSession.EventId into eventQuizSessions
                    from quizSession in eventQuizSessions.DefaultIfEmpty()
                    group new { @event, eventVoucher, quizSession } by @event.CounterPartId into counterpartGroup
                    select new CounterPartStatisticsEventsResponseDto
                    {
                        TotalEvents = counterpartGroup.Count(),
                        TotalVouchers = counterpartGroup.Count(g => g.eventVoucher != null),
                        TotalVouchersValue = counterpartGroup.Sum(g => g.eventVoucher != null ? g.eventVoucher.Value : 0),
                        TotalQuizSessions = counterpartGroup.Count(g => g.quizSession != null),
                        TotalShakeGame = counterpartGroup.Count(g => g.@event.ShakeVoucherId != null),
                    }
                )
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

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