using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.PlayerCommands.LikeEvent;

public class LikeEventHandler : IRequestHandler<LikeEventCommand, BaseResponse<EventDto>>
{
    private readonly ILogger<LikeEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public LikeEventHandler(ILogger<LikeEventHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }
    
    public async Task<BaseResponse<EventDto>> Handle(LikeEventCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(LikeEventHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        
        var response = new BaseResponse<EventDto>();
        try
        {
            var existedInFavourite = await _unitOfWork.FavoriteEvents
                .Where(x => x.EventId == request.EventId && x.PlayerId == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (existedInFavourite != null)
            {
                response.ToBadRequestResponse("Event already liked");
                return response;
            }
            //Add to favourite
            var newFavourite = new FavoriteEvent
            {
                EventId = request.EventId,
                PlayerId = userId,
            };
            await _unitOfWork.FavoriteEvents.AddAsync(newFavourite, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            //Response data
            var eventData = await _unitOfWork.Events
                .Where(x => x.Id == request.EventId)
                .Select(x => new EventDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            response.ToSuccessResponse(eventData);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
