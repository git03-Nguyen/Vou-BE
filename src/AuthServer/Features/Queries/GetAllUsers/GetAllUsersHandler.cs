using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace AuthServer.Features.Queries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, BaseResponse<List<UserDetailDto>>>
{
    private readonly ILogger<GetAllUsersHandler> _logger;
    private readonly UserManager<User> _userManager;
    public GetAllUsersHandler(ILogger<GetAllUsersHandler> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<BaseResponse<List<UserDetailDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<List<UserDetailDto>>();
        const string methodName = $"{nameof(GetAllUsersHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    AvatarUrl = u.AvatarUrl ?? Constants.DefaultAvatarUrl,
                    Role = u.Role
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            response.ToSuccessResponse(users);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}