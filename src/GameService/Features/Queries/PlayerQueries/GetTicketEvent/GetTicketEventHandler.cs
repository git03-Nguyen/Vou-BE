using GameService.Data.Models;
using GameService.DTOs;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventHandler : IRequestHandler<GetTicketEventQuery, BaseResponse<PlayerShakeDto>>
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

    public async Task<BaseResponse<PlayerShakeDto>> Handle(GetTicketEventQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetTicketEventHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<PlayerShakeDto>();
        
        try
        {
            var player = await
            (
                from playerInSession in _unitOfWork.PlayerShakeSessions.GetAll()
                join @event in _unitOfWork.Events.GetAll()
                    on playerInSession.EventId equals @event.Id
                where @event.Status == EventStatus.Approved || @event.Status == EventStatus.InProgress
                select playerInSession
            )
            .FirstOrDefaultAsync(cancellationToken);

            // If player is new to event
            if (player is null)
            {
                var newRow = new PlayerShakeSession
                {
                    EventId = request.EventId,
                    PlayerId = userId
                };

                await _unitOfWork.PlayerShakeSessions.AddAsync(newRow,cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var responseDat = new PlayerShakeDto
                {
                    Id = userId,
                    Tickets = newRow.Tickets,
                    Diamonds = newRow.Diamond
                };
                return response.ToSuccessResponse(responseDat);
            }
            
            // If player has already joined the event, check if the reset time has passed
            if (player.NextResetTicketsTime < DateTime.Now)
            {
                var newPlayer = new PlayerShakeSession();

                player.Tickets = newPlayer.Tickets;
                player.NextResetTicketsTime = newPlayer.NextResetTicketsTime;
                _unitOfWork.PlayerShakeSessions.Update(player);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            var responseData = new PlayerShakeDto
            {
                Id = userId,
                Tickets = player.Tickets,
                Diamonds = player.Diamond
            };
            
            return response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}