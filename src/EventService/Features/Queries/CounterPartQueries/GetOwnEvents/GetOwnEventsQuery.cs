using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEvents;

public class GetOwnEventsQuery : IRequest<BaseResponse<GetOwnEventsQueryResponse>>
{
}

public class GetOwnEventsQueryResponse
{
    public List<FullEventDto> Events { get; set; }
}