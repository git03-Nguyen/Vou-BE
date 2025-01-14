using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetFavoriteEvents;

public class GetFavoriteEventsHandler : IRequestHandler<GetFavoriteEventsQuery, BaseResponse<GetFavoriteEventsReponse>>
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

    public async Task<BaseResponse<GetFavoriteEventsReponse>> Handle(GetFavoriteEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetFavoriteEventsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetFavoriteEventsReponse>();

        try
        {
            var events = await
            (
                from player in _unitOfWork.Players.GetAll()
                join favorite in _unitOfWork.FavoriteEvents.GetAll()
                    on player.Id equals favorite.PlayerId
                join @event in _unitOfWork.Events.GetAll()
                    on favorite.EventId equals @event.Id
                join counterPart in _unitOfWork.CounterParts.GetAll()
                    on @event.CounterPartId equals counterPart.Id
                where !@event.IsDeleted 
                      && player.Id == userId
                      && !counterPart.IsBlocked
                      && @event.Status == EventStatus.Approved
                orderby @event.StartDate descending
                select new EventDto
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Description = @event.Description,
                    ImageUrl = @event.ImageUrl,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    Status = @event.Status,
                    CreatedDate = @event.CreatedDate,
                    CounterPart = new CounterPartDto
                    {
                        Id = counterPart.Id,
                        FullName = counterPart.FullName,
                        ImageUrl = counterPart.ImageUrl,
                        Address = counterPart.Address,
                        Field = counterPart.Field,
                    }
                    // Quiz and Shake game will be added later
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new GetFavoriteEventsReponse { Events = events };
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