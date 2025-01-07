using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace AuthServer.Features.Commands.RegisterForPlayer;

public class RegisterForPlayerHandler : IRequestHandler<RegisterForPlayerCommand, BaseResponse<UserShortDto>>
{
    private readonly ILogger<RegisterForPlayerHandler> _logger;
    private readonly UserManager<User> _userManager;
    public RegisterForPlayerHandler(ILogger<RegisterForPlayerHandler> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<BaseResponse<UserShortDto>> Handle(RegisterForPlayerCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserShortDto>();
        var methodName = $"{nameof(RegisterForPlayerHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
            // 1. Check if exists
            var existedUser = await _userManager.Users
                .Where(u => 
                    !u.IsDeleted 
                    && (string.Equals(u.NormalizedEmail, request.Email.ToUpper(), StringComparison.Ordinal) 
                        || string.Equals(u.NormalizedUserName, request.UserName.ToUpper(), StringComparison.Ordinal) 
                        || string.Equals(u.PhoneNumber, request.PhoneNumber, StringComparison.Ordinal)))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken); 
            if (existedUser is not null)
            {
                response.ToBadRequestResponse("User already exists");
                return response;
            }
            
            // 2. Create user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = Constants.PLAYER
            };
            
            // 3. Add user
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"{methodName} Failed to create user: {JsonSerializer.Serialize(result.Errors)}");
                response.ToBadRequestResponse("Failed to create user");
                return response;
            }
            var resultRole = await _userManager.AddToRoleAsync(user, Constants.PLAYER);
            if (!resultRole.Succeeded)
            {
                _logger.LogError($"{methodName} Failed to add role to user: {JsonSerializer.Serialize(resultRole.Errors)}");
                response.ToBadRequestResponse("Failed to add role to user");
                await _userManager.DeleteAsync(user);
                return response;
            }
            
            var responseData = new UserShortDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl ?? Constants.DefaultAvatarUrl,
                Role = user.Role
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