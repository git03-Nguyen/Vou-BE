using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.AdminQueries.GetAllEvents;

public class GetAllEventsQuery : IRequest<BaseResponse<GetAllEventQueryResponse>>
{
}

public class GetAllEventQueryResponse
{
    public List<EventDto> Events { get; set; }
}