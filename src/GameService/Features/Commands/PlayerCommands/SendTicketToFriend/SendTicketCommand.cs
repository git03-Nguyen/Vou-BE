using MediatR;
using Shared.Response;

namespace GameService.Features.Commands.PlayerCommands.SendTicketToFriend;

public class SendTicketCommand : IRequest<BaseResponse>
{
    public string PlayerId { get; set; }
    public string EventId { get; set; }
}

