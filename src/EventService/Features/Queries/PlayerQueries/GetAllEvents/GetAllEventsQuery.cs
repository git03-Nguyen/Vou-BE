using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetAllEvents;

public class GetAllEventsQuery : IRequest<BaseResponse<GetAllEventsReponse>>
{
}

public class GetAllEventsReponse
{
    public List<EventDto> Events { get; set; }
}