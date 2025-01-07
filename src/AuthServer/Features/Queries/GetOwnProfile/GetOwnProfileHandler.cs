using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Features.Queries.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Queries.GetOwnProfile;

public class GetOwnProfileHandler : IRequestHandler<GetOwnProfileQuery, BaseResponse<UserDetailDto>>
{
    private readonly ILogger<GetAllUsersHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    public GetOwnProfileHandler(ILogger<GetAllUsersHandler> logger, UserManager<User> userManager, ICustomHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<UserDetailDto>> Handle(GetOwnProfileQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserDetailDto>();
        var userId = _httpContextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnProfileHandler)}.{nameof(Handle)} UserId = {userId} =>";
        _logger.LogInformation(methodName);

        try
        {
            var user = await _userManager.Users
                .Where(u => !u.IsDeleted && u.Id == userId && !u.IsBlocked)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    AvatarUrl = u.AvatarUrl ?? Constants.DefaultAvatarUrl,
                    Role = u.Role
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            if (user is null)
            {
                response.ToNotFoundResponse();
                return response;
            }
            
            response.ToSuccessResponse(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}