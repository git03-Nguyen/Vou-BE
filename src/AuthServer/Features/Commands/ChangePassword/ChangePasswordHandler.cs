using System.Security.Claims;
using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<object>>
{
    private readonly ILogger<ChangePasswordHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public ChangePasswordHandler(ILogger<ChangePasswordHandler> logger, UserManager<User> userManager, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<object>();
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(ChangePasswordHandler)}.{nameof(Handle)} UserId = {userId}, Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                response.ToBadRequestResponse("Cannot find user");
                return response;
            }
            
            var isCorrectOldPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            if (!isCorrectOldPassword)
            {
                response.ToBadRequestResponse("Password is incorrect");
                return response;
            }
            
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                response.ToInternalErrorResponse("Internal server error");
                return response;
            }
            
            response.ToSuccessResponse(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}