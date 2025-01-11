using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Queries.AdminQueries.GetAllUsers;

public class GetAllUsersQuery : IRequest<BaseResponse<GetAllUsersResponse>>
{
}

public class GetAllUsersResponse
{
    public List<UserFullProfileDto> Users { get; set; }
}