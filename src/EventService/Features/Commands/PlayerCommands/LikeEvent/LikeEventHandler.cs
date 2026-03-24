using System.Data;
using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            await using var transaction = await _unitOfWork.OpenTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var eventData = await _unitOfWork.Events
                .Where(x => x.Id == request.EventId && !x.IsDeleted)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (eventData == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToNotFoundResponse("Event not found");
                return response;
            }

            var existedInFavourite = await _unitOfWork.FavoriteEvents
                .Where(x => x.EventId == request.EventId && x.PlayerId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existedInFavourite != null && !existedInFavourite.IsDeleted)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Event already liked");
                return response;
            }

            if (existedInFavourite != null)
            {
                existedInFavourite.IsDeleted = false;
                existedInFavourite.DeletedDate = null;
                existedInFavourite.ModifiedDate = DateTime.UtcNow;
                _unitOfWork.FavoriteEvents.Update(existedInFavourite);
            }
            else
            {
                // Add to favourite
                var newFavourite = new FavoriteEvent
                {
                    EventId = request.EventId,
                    PlayerId = userId,
                };
                await _unitOfWork.FavoriteEvents.AddAsync(newFavourite, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            response.ToSuccessResponse(eventData);
        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.SerializationFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Like event serialization conflict");
            response.ToBadRequestResponse("Event already liked");
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Like event serialization conflict during save");
            response.ToBadRequestResponse("Event already liked");
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(e, methodName);
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
