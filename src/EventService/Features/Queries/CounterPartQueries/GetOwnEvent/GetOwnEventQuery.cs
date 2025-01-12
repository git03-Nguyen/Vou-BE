using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEvent;

public class GetOwnEventQuery : IRequest<BaseResponse<FullEventDto>>
{
    public string EventId { get; set; }
}