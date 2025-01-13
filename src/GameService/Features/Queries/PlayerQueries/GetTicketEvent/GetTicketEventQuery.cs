using MediatR;
using Shared.Response;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventQuery : IRequest<BaseResponse<object>>
{
    public string EventId;
}

