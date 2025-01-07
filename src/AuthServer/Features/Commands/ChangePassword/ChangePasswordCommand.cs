using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<BaseResponse<object>>
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}