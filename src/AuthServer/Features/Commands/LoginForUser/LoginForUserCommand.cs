using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.LoginForUser;

public class LoginForUserCommand : IRequest<BaseResponse<LoginSuccessDto>>
{
    public string EmailOrUserName { get; set; }
    public string Password { get; set; }
}