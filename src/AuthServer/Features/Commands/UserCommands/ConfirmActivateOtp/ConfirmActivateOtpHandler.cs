using System.Text.Json;
using AuthServer.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Response;

namespace AuthServer.Features.Commands.UserCommands.ConfirmActivateOtp;

public class ConfirmActivateOtpHandler : IRequestHandler<ConfirmActivateOtpCommand, BaseResponse<object>>
{
    private readonly ILogger<ConfirmActivateOtpHandler> _logger;
    private readonly UserManager<User> _userManager;
    public ConfirmActivateOtpHandler(ILogger<ConfirmActivateOtpHandler> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<BaseResponse<object>> Handle(ConfirmActivateOtpCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<object>();
        var methodName = $"{nameof(ConfirmActivateOtpHandler)}.{nameof(Handle)}, Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var isEmail= request.UserNameOrEmail.Contains("@");
            var user = isEmail
                ? await _userManager.FindByEmailAsync(request.UserNameOrEmail)
                : await _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (user is null)
            {
                response.ToBadRequestResponse("User not found");
                return response;
            }
            //Check OTP
            if (user.OtpActivateCode != request.Otp)
            {
                response.ToBadRequestResponse("Invalid OTP");
                return response;
            }
            else if (user.OtpActivateExpiredTime < DateTime.Now)
            {
                response.ToBadRequestResponse("OTP expired");
                return response;
            }
            
            //Activate user
            user.EmailConfirmed = true;
            
            //Update user
            await _userManager.UpdateAsync(user);
            
            response.ToSuccessResponse(null, "Activate successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}