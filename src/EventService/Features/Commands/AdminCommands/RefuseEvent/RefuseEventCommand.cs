using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.AdminCommands.RefuseEvent;

public class RefuseEventCommand : IRequest<BaseResponse<EventStatusDto>>
{
    public string EventId { get; set; }
    public RefuseEventCommand(string eventId)
    {
        EventId = eventId;
    }
}