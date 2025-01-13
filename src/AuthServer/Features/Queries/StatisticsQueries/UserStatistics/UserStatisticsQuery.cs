using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Queries.StatisticsQueries.UserStatistics;

public class UserStatisticsQuery : IRequest<BaseResponse<UserStatisticsResponseDto>>
{
}
