using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEventStatistics;

public class GetOwnEventStatisticsQuery : IRequest<BaseResponse<CounterPartStatisticsEventsResponseDto>>
{
    public string EventId { get; set; }
}