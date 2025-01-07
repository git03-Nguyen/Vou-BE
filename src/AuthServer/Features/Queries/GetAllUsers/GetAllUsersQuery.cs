using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<BaseResponse<List<UserDetailDto>>>
{
}