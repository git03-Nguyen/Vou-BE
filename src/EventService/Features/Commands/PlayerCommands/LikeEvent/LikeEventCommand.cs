using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.PlayerCommands.LikeEvent;

public class LikeEventCommand : IRequest<BaseResponse<EventDto>>
{
    public string EventId { get; set; }
}