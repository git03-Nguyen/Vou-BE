using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.AdminCommands.UnblockUser;

public class UnblockUserCommand : IRequest<BaseResponse<BlockUserResponseDto>>
{
    public string EmailOrUserName { get; set; }
}