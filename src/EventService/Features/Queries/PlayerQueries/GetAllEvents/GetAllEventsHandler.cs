using EventService.DTOs;
using EventService.Enums;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
                    where e.Status == EventStatus.Approved || e.Status == EventStatus.InProgress
                        orderby e.StartDate descending
                        select new EventDto
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
                                Field = ctp.Field
                            }
                        }
                
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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