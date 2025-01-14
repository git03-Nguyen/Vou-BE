using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.StatisticsQueries.EventStatistics;

public class EventStatisticsQuery : IRequest<BaseResponse<EventStatisticsResponseDto>>
{
  
}
