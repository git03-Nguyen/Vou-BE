using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetFavoriteEvents;

public class GetFavoriteEventsQuery : IRequest<BaseResponse<GetFavoriteEventsReponse>>
{
}

public class GetFavoriteEventsReponse
{
    public List<EventDto> Events { get; set; }
}