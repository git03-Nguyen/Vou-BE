using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.BlockUser;

public class BlockUserCommand : IRequest<BaseResponse<UserShortDto>>
{
    public string EmailOrUserName { get; set; }
}