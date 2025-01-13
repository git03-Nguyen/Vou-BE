using GameService.DTOs;
using MediatR;
using Shared.Response;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventQuery : IRequest<BaseResponse<PlayerShakeDto>>
{
    public string EventId;

    public GetTicketEventQuery(string eventId)
    {
        EventId = eventId;
    }
}

