using System.Security.Claims;
using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Response;

namespace AuthServer.Features.Commands.LoginForUser;

public class LoginForUserHandler : IRequestHandler<LoginForUserCommand, BaseResponse<LoginSuccessDto>>
{
    private readonly ILogger<LoginForUserHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
    public LoginForUserHandler(ILogger<LoginForUserHandler> logger, UserManager<User> userManager, ITokenService tokenService, IUserClaimsPrincipalFactory<User> claimsFactory)
    {
        _logger = logger;
        _userManager = userManager;
        _tokenService = tokenService;
        _claimsFactory = claimsFactory;
    }

    public async Task<BaseResponse<LoginSuccessDto>> Handle(LoginForUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<LoginSuccessDto>();
        var methodName = $"{nameof(LoginForUserHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
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
                response.ToUnauthorizedResponse();
                return response;
            }

            if (user.IsBlocked)
            {
                response.ToForbiddenResponse("User is blocked");
                return response;
            }
                
            // 2. Check password is valid
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                response.ToUnauthorizedResponse();
                return response;
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
            var isAdmin = string.Equals(role, Constants.ADMIN, StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                // 3. Check user has not confirmed email yet
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail)
                {
                    response.ToUnauthorizedResponse("User has not confirmed email yet");
                    return response;
                }
            
                // 4. Check user is locked
                var isLockedOut = user is { LockoutEnabled: true, LockoutEnd: not null } &&
                                  user.LockoutEnd.Value > DateTimeOffset.Now; 
                if (isLockedOut)
                {
                    response.ToUnauthorizedResponse($"User is locked until {user.LockoutEnd.ToString()}"); 
                    return response;
                }
            }
            
            // 6. Generate token
            var principal = await _claimsFactory.CreateAsync(user);
            var tokenDescriptor = new TokenCreationRequest
            {
                Subject = principal,
                ValidatedRequest = new ValidatedRequest
                {
                    ClientClaims =
                    {
                        new Claim(JwtClaimTypes.Id, user.Id),
                        new Claim(JwtClaimTypes.Name, user.UserName ?? string.Empty),
                        new Claim(JwtClaimTypes.Email, user.Email ?? string.Empty),
                        new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber ?? string.Empty),
                        new Claim(JwtClaimTypes.Role, role),
                    }
                }
            };
            var accessToken = await _tokenService.CreateAccessTokenAsync(tokenDescriptor);
            var tokenResult = await _tokenService.CreateSecurityTokenAsync(accessToken);
            
            var responseData = new LoginSuccessDto
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