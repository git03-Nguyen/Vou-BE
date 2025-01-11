using System.Text.Json;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Commands.AdminCommands.UnblockUser;

public class UnblockUserHandler : IRequestHandler<UnblockUserCommand, BaseResponse<BlockUserResponseDto>>
{
    private readonly ILogger<UnblockUserHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    public UnblockUserHandler(ILogger<UnblockUserHandler> logger, UserManager<User> userManager, ICustomHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<BlockUserResponseDto>> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<BlockUserResponseDto>();
        var methodName = string.Empty;

        try
        {
            var adminId = _httpContextAccessor.GetCurrentUserId();
            methodName = $"{nameof(UnblockUserHandler)}.{nameof(Handle)} Admin = {adminId}, Request = {JsonSerializer.Serialize(request)} =>";
            _logger.LogInformation(methodName);
            
            // 1. Check user exists
            var isEmailOrUserName = request.EmailOrUserName.Contains('@');
            var user = isEmailOrUserName
                ? await _userManager.FindByEmailAsync(request.EmailOrUserName)
                : await _userManager.FindByNameAsync(request.EmailOrUserName);
            if (user is null || user.IsDeleted)
            {
                response.ToBadRequestResponse("User not found");
                return response;
            }

            if (!user.IsBlocked)
            {
                response.ToBadRequestResponse("User is not blocked");
                return response;
            }
                
            // 2. Unblock user
            user.IsBlocked = false;
            user.BlockedDate = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"{methodName} Unblock user failed. Error = {JsonSerializer.Serialize(result.Errors)}");
                response.ToInternalErrorResponse("Unblock user failed");
                return response;
            }

            var responseData = new BlockUserResponseDto
            {
                IsBlocked = user.IsBlocked,
                BlockedDate = user.BlockedDate.Value
            };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}