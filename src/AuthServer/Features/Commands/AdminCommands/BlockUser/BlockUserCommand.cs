using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.AdminCommands.BlockUser;

public class BlockUserCommand : IRequest<BaseResponse<BlockUserResponseDto>>
{
    public string EmailOrUserName { get; set; }
}