using System.Text.Json;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEvent;

public class GetOwnEventHandler : IRequestHandler<GetOwnEventQuery, BaseResponse<FullEventDto>>
{
    private readonly ILogger<GetOwnEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetOwnEventHandler(ILogger<GetOwnEventHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<FullEventDto>> Handle(GetOwnEventQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnEventHandler)}.{nameof(Handle)} UserId: {userId}, EventId: {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<FullEventDto>();

        try
        {
            var @event = await
            (
                // Get events along with its shake session and voucher
                from _event in _unitOfWork.Events.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on _event.ShakeVoucherId equals voucher.Id into eventVouchers
                from eventVoucher in eventVouchers.DefaultIfEmpty()
                where !_event.IsDeleted && _event.CounterPartId == userId && _event.Id == request.EventId
                let hasShakeSession = _event.ShakeVoucherId != null
                select new FullEventDto
                {
                    Id = _event.Id,
                    Name = _event.Name,
                    Description = _event.Description,
                    ImageUrl = _event.ImageUrl,
                    StartDate = _event.StartDate,
                    Status = _event.Status,
                    CreatedDate = _event.CreatedDate,
                    ShakeSession = hasShakeSession ? new ShakeSessionDto
                    {
                        Price = _event.ShakePrice ?? 0,
                        AverageDiamond = _event.ShakeAverageDiamond ?? 0,
                        WinRate = _event.ShakeWinRate ?? 0,
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
            .FirstOrDefaultAsync(cancellationToken);
            
            if (@event is null)
            {
                response.ToNotFoundResponse();
                return response;
            }
            
            // Populate events with .QuizSessions
            @event.QuizSessions = await 
            (
                from quizSession in _unitOfWork.QuizSessions.GetAll()
                join event_ in _unitOfWork.Events.GetAll()
                    on quizSession.EventId equals event_.Id
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on event_.ShakeVoucherId equals voucher.Id
                join quizSet in _unitOfWork.QuizSets.GetAll()
                    on quizSession.QuizSetId equals quizSet.Id
                where quizSession.EventId == request.EventId
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

            response.ToSuccessResponse(@event);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}