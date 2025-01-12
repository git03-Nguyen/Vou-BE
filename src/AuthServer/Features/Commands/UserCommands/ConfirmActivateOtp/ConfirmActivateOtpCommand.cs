using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.UserCommands.ConfirmActivateOtp;

public class ConfirmActivateOtpCommand : IRequest<BaseResponse<object>>
{
    public string UserNameOrEmail { get; set; }
    public string Otp { get; set; }
}