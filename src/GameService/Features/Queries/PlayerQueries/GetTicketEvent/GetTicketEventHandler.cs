using GameService.Data.Models;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Contracts.EventMessages;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventHandler : IRequestHandler<GetTicketEventQuery, BaseResponse<object>>
{
    private readonly ILogger<GetTicketEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetTicketEventHandler(ILogger<GetTicketEventHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<object>> Handle(GetTicketEventQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetTicketEventHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<object>();
        
        try
        {
            var player = await
            (
                from playerInSession in _unitOfWork.PlayerShakeSessions.GetAll()
                join @event in _unitOfWork.Events.GetAll()
                    on playerInSession.EventId equals @event.Id
                where @event.Status == EventStatus.Approved || @event.Status == EventStatus.InProgress
                
                select new
                {
                    playerInSession.Tickets
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

            if (player is null)
            {
                var newRow = new PlayerShakeSession
                {
                    EventId = request.EventId,
                    PlayerId = userId
                };

                await _unitOfWork.PlayerShakeSessions.AddAsync(newRow,cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return response.ToSuccessResponse(5);
            }
            else
            {
                return response.ToSuccessResponse(player.Tickets);
            }
            

            // var events = await
            //         (
            //             from player in _unitOfWork.Players.GetAll()
            //             join favorite in _unitOfWork.FavoriteEvents.GetAll()
            //                 on player.Id equals favorite.PlayerId
            //             join @event in _unitOfWork.Events.GetAll()
            //                 on favorite.EventId equals @event.Id
            //             join counterPart in _unitOfWork.CounterParts.GetAll()
            //                 on @event.CounterPartId equals counterPart.Id
            //             where !@event.IsDeleted 
            //                   && player.Id == userId
            //                   && !counterPart.IsBlocked
            //                   && @event.Status == EventStatus.Approved
            //             orderby @event.StartDate descending
            //             select new EventDto
            //             {
            //                 Id = @event.Id,
            //                 Name = @event.Name,
            //                 Description = @event.Description,
            //                 ImageUrl = @event.ImageUrl,
            //                 StartDate = @event.StartDate,
            //                 Status = @event.Status,
            //                 CreatedDate = @event.CreatedDate,
            //                 CounterPart = new CounterPartDto
            //                 {
            //                     Id = counterPart.Id,
            //                     FullName = counterPart.FullName,
            //                     ImageUrl = counterPart.ImageUrl,
            //                     Address = counterPart.Address,
            //                     Field = counterPart.Field,
            //                 }
            //                 // Quiz and Shake game will be added later
            //             }
            //         )
            //         .AsNoTracking()
            //         .ToListAsync(cancellationToken);
            //
            // .AsNoTracking()
            // .ToListAsync(cancellationToken);

            // var responseData = new object { Events = events };
            // response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}