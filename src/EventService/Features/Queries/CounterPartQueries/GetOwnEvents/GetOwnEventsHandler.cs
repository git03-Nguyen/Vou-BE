using System.Text.Json;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEvents;

public class GetOwnEventsHandler : IRequestHandler<GetOwnEventsQuery, BaseResponse<GetOwnEventsQueryResponse>>
{
    private readonly ILogger<GetOwnEventsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetOwnEventsHandler(ILogger<GetOwnEventsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetOwnEventsQueryResponse>> Handle(GetOwnEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnEventsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetOwnEventsQueryResponse>();

        try
        {
            var events = await
            (
                // Get events along with its shake session and voucher
                from @event in _unitOfWork.Events.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on @event.ShakeVoucherId equals voucher.Id into eventVouchers
                from eventVoucher in eventVouchers.DefaultIfEmpty()
                where !@event.IsDeleted && @event.CounterPartId == userId
                let hasShakeSession = @event.ShakeVoucherId != null
                select new FullEventDto
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Description = @event.Description,
                    ImageUrl = @event.ImageUrl,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    Status = @event.Status,
                    CreatedDate = @event.CreatedDate,
                    ShakeSession = hasShakeSession ? new ShakeSessionDto
                    {
                        Price = @event.ShakePrice ?? 0,
                        AverageDiamond = @event.ShakeAverageDiamond ?? 0,
                        WinRate = @event.ShakeWinRate ?? 0,
                        Voucher = new VoucherDto
                        {
                            Id = eventVoucher.Id,
                            Title = eventVoucher.Title,
                            ImageUrl = eventVoucher.ImageUrl,
                            Value = eventVoucher.Value,
                        }
                    } : null
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            
            if (events.Count == 0)
            {
                var responseDat = new GetOwnEventsQueryResponse { Events = events };
                response.ToSuccessResponse(responseDat);
                return response;
            }
            
            // Populate events with .QuizSessions
            var eventIds = events.Select(e => e.Id).ToList();
            var quizSessions = await 
            (
                from quizSession in _unitOfWork.QuizSessions.GetAll()
                join event_ in _unitOfWork.Events.GetAll()
                    on quizSession.EventId equals event_.Id
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on quizSession.VoucherId equals voucher.Id
                join quizSet in _unitOfWork.QuizSets.GetAll()
                    on quizSession.QuizSetId equals quizSet.Id
                where eventIds.Contains(quizSession.EventId)
                select new QuizSessionDto
                {
                    Id = quizSession.Id,
                    TakeTop = quizSession.TakeTop,
                    StartTime = quizSession.StartTime,
                    Voucher = new VoucherDto
                    {
                        Id = quizSession.VoucherId,
                        Title = voucher.Title,
                        ImageUrl = voucher.ImageUrl,
                        Value = voucher.Value,
                    },
                    QuizSet = new QuizSetDto
                    {
                        Id = quizSet.Id,
                        Title = quizSet.Title,
                        ImageUrl = quizSet.ImageUrl,
                        QuizesSerialized = quizSet.QuizesSerialized
                    }
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            
            foreach (var @event in events)
            {
                @event.QuizSessions = quizSessions
                    .Where(qs => qs.EventId == @event.Id)
                    .ToList();
            }

            var responseData = new GetOwnEventsQueryResponse { Events = events };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}