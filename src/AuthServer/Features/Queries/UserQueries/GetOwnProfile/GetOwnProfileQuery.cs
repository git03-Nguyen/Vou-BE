using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Queries.UserQueries.GetOwnProfile;

public class GetOwnProfileQuery : IRequest<BaseResponse<UserFullProfileDto>>
{
}