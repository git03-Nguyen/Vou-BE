using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Queries.GetOwnProfile;

public class GetOwnProfileQuery : IRequest<BaseResponse<UserDetailDto>>
{
}