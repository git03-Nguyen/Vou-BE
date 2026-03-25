using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetEventDetail;

public class GetEventDetailQuery : IRequest<BaseResponse<FullEventDto>>
{
    public string EventId { get; set; } = string.Empty;
}
