using EventService.DTOs;
using EventService.Features.Queries.AdminQueries.GetAllEvents;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;

namespace EventService.Features.Commands.AdminCommands.RefuseEvent;

public class RefuseEventHandler : IRequestHandler<RefuseEventCommand, BaseResponse<EventStatusDto>>
{
    private readonly ILogger<GetAllEventsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public RefuseEventHandler(ILogger<GetAllEventsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<EventStatusDto>> Handle(RefuseEventCommand request, CancellationToken cancellationToken)
    {
        var methodName = $"{nameof(GetAllEventsHandler)}.{nameof(Handle)} EventId: {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<EventStatusDto>();

        try
        {
            var thisEvent = await _unitOfWork.Events
                .Where(x => !x.IsDeleted
                            && x.Id == request.EventId
                            && x.Status == EventStatus.Pending)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (thisEvent is null)
            {
                response.ToNotFoundResponse();
                return response;
            }

            thisEvent.Status = EventStatus.Canceled;
            _unitOfWork.Events.Update(thisEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var responseData = new EventStatusDto
            {
                Id = thisEvent.Id,
                Status = thisEvent.Status
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