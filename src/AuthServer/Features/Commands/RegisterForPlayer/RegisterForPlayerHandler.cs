using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace AuthServer.Features.Commands.RegisterForPlayer;

public class RegisterForPlayerHandler : IRequestHandler<RegisterForPlayerCommand, BaseResponse<UserProfileDto>>
{
    private readonly ILogger<RegisterForPlayerHandler> _logger;
    private readonly UserManager<User> _userManager;
    public RegisterForPlayerHandler(ILogger<RegisterForPlayerHandler> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<BaseResponse<UserProfileDto>> Handle(RegisterForPlayerCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserProfileDto>();
        var methodName = $"{nameof(RegisterForPlayerHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
            // 1. Check if exists
            var existedUser = await _userManager.Users
                .Where(u => !u.IsDeleted &&
                    string.Equals(u.NormalizedEmail, request.Email.ToUpper(), StringComparison.Ordinal) 
                               || string.Equals(u.NormalizedUserName, request.UserName.ToUpper(), StringComparison.Ordinal) 
                               || string.Equals(u.PhoneNumber, request.PhoneNumber, StringComparison.Ordinal))
                .FirstOrDefaultAsync(cancellationToken); 
            if (existedUser is not null)
            {
                response.ToBadRequestResponse("User already exists");
                return response;
            }
            
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = Constants.PLAYER
            };
            
            var responseData = new UserProfileDto
            {
                AccessToken = tokenResult,
                LifeTime = accessToken.Lifetime
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