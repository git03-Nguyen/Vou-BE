using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.AdminCommands.AcceptEvent;

public class AcceptEventCommand : IRequest<BaseResponse<EventStatusDto>>
{
    public string EventId { get; set; }
    public AcceptEventCommand(string eventId)
    {
        EventId = eventId;
    }
}