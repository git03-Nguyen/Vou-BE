using System.Security.Claims;
using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using IdentityModel;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Response;

namespace AuthServer.Features.Commands.BlockUser;

public class BlockUserHandler : IRequestHandler<BlockUserCommand, BaseResponse<UserShortDto>>
{
    private readonly ILogger<BlockUserHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IdentityServerTools _identityServerTools;
    private readonly AuthenticationOptions _authenticationOptions;
    public BlockUserHandler(ILogger<BlockUserHandler> logger, UserManager<User> userManager, IdentityServerTools identityServerTools, IOptions<AuthenticationOptions> authenticationOptions)
    {
        _logger = logger;
        _userManager = userManager;
        _identityServerTools = identityServerTools;
        _authenticationOptions = authenticationOptions.Value;
    }

    public async Task<BaseResponse<UserShortDto>> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserShortDto>();
        var methodName = $"{nameof(BlockUserHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
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

            if (user.IsBlocked)
            {
                response.ToForbiddenResponse("User is already blocked");
                return response;
            }
                
            // 2. Block user
            user.IsBlocked = true;
            user.BlockedDate = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.ToBadRequestResponse("Block user failed");
                return response;
            }

            response.ToSuccessResponse();

        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}