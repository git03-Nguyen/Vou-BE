using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetAllEvents;

public class GetFavoriteEventsHandler : IRequestHandler<GetAllEventsQuery, BaseResponse<GetAllEventsReponse>>
{
    private readonly ILogger<GetFavoriteEventsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetFavoriteEventsHandler(ILogger<GetFavoriteEventsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetAllEventsReponse>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetFavoriteEventsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetAllEventsReponse>();

        try
        {
            var events = await
            (
                from e in _unitOfWork.Events.GetAll()
                join ctp in _unitOfWork.CounterParts.GetAll()
                    on e.CounterPartId equals ctp.Id
                join v in _unitOfWork.Vouchers.GetAll()
                    on e.ShakeVoucherId equals v.Id into groupVou
                from v in groupVou.DefaultIfEmpty()
                where !e.IsDeleted
                      && (e.Status == EventStatus.Approved 
                          || e.Status == EventStatus.InProgress)
                orderby e.StartDate descending
                let hasShakeSession = v != null
                select new FullEventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Status = e.Status,
                    CreatedDate = e.CreatedDate,
                    CounterPart = new CounterPartDto
                    {
                        Id = ctp.Id,
                        FullName = ctp.FullName,
                        ImageUrl = ctp.ImageUrl,
                        Address = ctp.Address,
                        Field = ctp.Field,
                    },
                    ShakeSession = hasShakeSession ? new ShakeSessionDto
                    {
                        Price = e.ShakePrice ?? 0,
                        AverageDiamond = e.ShakeAverageDiamond ?? 0,
                        WinRate = e.ShakeWinRate ?? 0,
                        Voucher = new VoucherDto
                        {
                            Id = v.Id,
                            Title = v.Title,
                            ImageUrl = v.ImageUrl,
                            Value = v.Value
                        }
                    } : null,
                    QuizSessions = null
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            
            // Get the list of eventId
            var eventIds = events.Select(e => e.Id).ToList();
            
            // Get the list of quiz sessions
            var quizSessions = await
                (
                    from qs in _unitOfWork.QuizSessions.GetAll()
                    join v in _unitOfWork.Vouchers.GetAll()
                        on qs.VoucherId equals v.Id
                    join q in _unitOfWork.QuizSets.GetAll()
                        on qs.QuizSetId equals q.Id
                    where !qs.IsDeleted
                          && eventIds.Contains(qs.EventId)
                    select new QuizSessionDto
                    {
                        Id = qs.Id,
                        EventId = qs.EventId,
                        StartTime = qs.StartTime,
                        TakeTop = qs.TakeTop,
                        Voucher = new VoucherDto
                        {
                            Id = v.Id,
                            Title = v.Title,
                            ImageUrl = v.ImageUrl,
                            Value = v.Value
                        },
                        QuizSet = null
                    }
                )
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            // Populate the quiz sessions to the events
            foreach (var e in events)
            {
                e.QuizSessions = quizSessions.Where(qs => qs.EventId == e.Id).ToList();
            }

            var responseData = new GetAllEventsReponse { Events = events };
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