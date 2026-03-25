using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace EventService.Features.Queries.AdminQueries.GetAllEvents;

public class GetAllEventsHandler : IRequestHandler<GetAllEventsQuery, BaseResponse<GetAllEventQueryResponse>>
{
    private readonly ILogger<GetAllEventsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetAllEventsHandler(ILogger<GetAllEventsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GetAllEventQueryResponse>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        const string methodName = $"{nameof(GetAllEventsHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetAllEventQueryResponse>();

        try
        {
            var search = request.Search?.Trim().ToLowerInvariant();

            var events = await
            (
                from event_ in _unitOfWork.Events.GetAll()
                join counterPart in _unitOfWork.CounterParts.GetAll()
                    on event_.CounterPartId equals counterPart.Id
                where !event_.IsDeleted
                      && (string.IsNullOrWhiteSpace(request.CounterPartId) || event_.CounterPartId == request.CounterPartId)
                      && (!request.Status.HasValue || event_.Status == request.Status.Value)
                      && (string.IsNullOrWhiteSpace(search)
                          || event_.Name.ToLower().Contains(search)
                          || event_.Description.ToLower().Contains(search)
                          || counterPart.FullName.ToLower().Contains(search)
                          || counterPart.Field.ToLower().Contains(search))
                orderby event_.CreatedDate descending, event_.StartDate descending
                let voucherCount = _unitOfWork.Vouchers.GetAll()
                    .Count(v => !v.IsDeleted && v.CounterPartId == event_.CounterPartId)
                let issuedVoucherCount = _unitOfWork.VoucherToPlayers.GetAll()
                    .Count(vp => !vp.IsDeleted && vp.EventId == event_.Id)
                let redeemedVoucherCount = _unitOfWork.VoucherToPlayers.GetAll()
                    .Count(vp => !vp.IsDeleted && vp.EventId == event_.Id && vp.UsedDate != null)
                let quizSessionCount = _unitOfWork.QuizSessions.GetAll()
                    .Count(qs => !qs.IsDeleted && qs.EventId == event_.Id)
                select new AdminEventDto
                {
                    Id = event_.Id,
                    Name = event_.Name,
                    Description = event_.Description,
                    ImageUrl = event_.ImageUrl,
                    StartDate = event_.StartDate,
                    EndDate = event_.EndDate,
                    Status = event_.Status,
                    CreatedDate = event_.CreatedDate,
                    CounterPart = new CounterPartDto
                    {
                        Id = counterPart.Id,
                        FullName = counterPart.FullName,
                        ImageUrl = counterPart.ImageUrl,
                        Address = counterPart.Address,
                        Field = counterPart.Field,
                    },
                    VoucherCount = voucherCount,
                    IssuedVoucherCount = issuedVoucherCount,
                    RedeemedVoucherCount = redeemedVoucherCount,
                    HasShakeGame = event_.ShakeVoucherId != null,
                    QuizSessionCount = quizSessionCount
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            if (request.HasVoucherInventory.HasValue)
            {
                events = events
                    .Where(x => request.HasVoucherInventory.Value
                        ? x.VoucherCount > 0 || x.IssuedVoucherCount > 0
                        : x.VoucherCount == 0 && x.IssuedVoucherCount == 0)
                    .ToList();
            }

            var responseData = new GetAllEventQueryResponse
            {
                TotalCount = events.Count,
                Events = events
            };
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
