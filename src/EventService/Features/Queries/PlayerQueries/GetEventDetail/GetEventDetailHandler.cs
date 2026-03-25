using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetEventDetail;

public class GetEventDetailHandler : IRequestHandler<GetEventDetailQuery, BaseResponse<FullEventDto>>
{
    private readonly ILogger<GetEventDetailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;

    public GetEventDetailHandler(
        ILogger<GetEventDetailHandler> logger,
        IUnitOfWork unitOfWork,
        ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<FullEventDto>> Handle(GetEventDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetEventDetailHandler)}.{nameof(Handle)} UserId = {userId}, EventId = {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<FullEventDto>();

        try
        {
            var eventDetail = await (
                from e in _unitOfWork.Events.GetAll()
                join counterPart in _unitOfWork.CounterParts.GetAll()
                    on e.CounterPartId equals counterPart.Id
                join shakeVoucher in _unitOfWork.Vouchers.GetAll()
                    on e.ShakeVoucherId equals shakeVoucher.Id into shakeVoucherJoin
                from shakeVoucher in shakeVoucherJoin.DefaultIfEmpty()
                where !e.IsDeleted
                      && e.Id == request.EventId
                      && (e.Status == EventStatus.Approved || e.Status == EventStatus.InProgress)
                select new
                {
                    Event = e,
                    CounterPart = counterPart,
                    ShakeVoucher = shakeVoucher,
                    IsFavorite = _unitOfWork.FavoriteEvents.GetAll()
                        .Any(fe => !fe.IsDeleted && fe.EventId == e.Id && fe.PlayerId == userId)
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

            if (eventDetail is null)
            {
                response.ToNotFoundResponse("Event not found");
                return response;
            }

            var quizSessions = await (
                from quizSession in _unitOfWork.QuizSessions.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on quizSession.VoucherId equals voucher.Id
                join quizSet in _unitOfWork.QuizSets.GetAll()
                    on quizSession.QuizSetId equals quizSet.Id
                where !quizSession.IsDeleted
                      && !voucher.IsDeleted
                      && !quizSet.IsDeleted
                      && quizSession.EventId == request.EventId
                orderby quizSession.StartTime ascending
                select new QuizSessionDto
                {
                    Id = quizSession.Id,
                    EventId = quizSession.EventId,
                    StartTime = quizSession.StartTime,
                    TakeTop = quizSession.TakeTop,
                    Voucher = new VoucherDto
                    {
                        Id = voucher.Id,
                        Title = voucher.Title,
                        ImageUrl = voucher.ImageUrl,
                        Value = voucher.Value,
                        TotalQuantity = voucher.TotalQuantity,
                        ExpiredDate = voucher.ExpiredDate,
                        RedemptionInstructions = voucher.RedemptionInstructions
                    },
                    QuizSet = new QuizSetDto
                    {
                        Id = quizSet.Id,
                        Title = quizSet.Title,
                        ImageUrl = quizSet.ImageUrl
                    }
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var fullEvent = new FullEventDto
            {
                Id = eventDetail.Event.Id,
                Name = eventDetail.Event.Name,
                Description = eventDetail.Event.Description,
                ImageUrl = eventDetail.Event.ImageUrl,
                StartDate = eventDetail.Event.StartDate,
                EndDate = eventDetail.Event.EndDate,
                Status = eventDetail.Event.Status,
                CreatedDate = eventDetail.Event.CreatedDate,
                CounterPart = new CounterPartDto
                {
                    Id = eventDetail.CounterPart.Id,
                    FullName = eventDetail.CounterPart.FullName,
                    Address = eventDetail.CounterPart.Address,
                    Field = eventDetail.CounterPart.Field,
                    ImageUrl = eventDetail.CounterPart.ImageUrl
                },
                ShakeSession = eventDetail.ShakeVoucher is null
                    ? null
                    : new ShakeSessionDto
                    {
                        Price = eventDetail.Event.ShakePrice ?? 0,
                        AverageDiamond = eventDetail.Event.ShakeAverageDiamond ?? 0,
                        WinRate = eventDetail.Event.ShakeWinRate ?? 0,
                        Voucher = new VoucherDto
                        {
                            Id = eventDetail.ShakeVoucher.Id,
                            Title = eventDetail.ShakeVoucher.Title,
                            ImageUrl = eventDetail.ShakeVoucher.ImageUrl,
                            Value = eventDetail.ShakeVoucher.Value,
                            TotalQuantity = eventDetail.ShakeVoucher.TotalQuantity,
                            ExpiredDate = eventDetail.ShakeVoucher.ExpiredDate,
                            RedemptionInstructions = eventDetail.ShakeVoucher.RedemptionInstructions
                        }
                    },
                QuizSessions = quizSessions
            };

            response.ToSuccessResponse(fullEvent, eventDetail.IsFavorite ? "favorite" : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
