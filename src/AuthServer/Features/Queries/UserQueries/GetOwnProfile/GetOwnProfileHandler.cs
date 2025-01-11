using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Features.Queries.AdminQueries.GetAllUsers;
using AuthServer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Queries.UserQueries.GetOwnProfile;

public class GetOwnProfileHandler : IRequestHandler<GetOwnProfileQuery, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<GetAllUsersHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    public GetOwnProfileHandler(ILogger<GetAllUsersHandler> logger, UserManager<User> userManager, ICustomHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(GetOwnProfileQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        var userId = _httpContextAccessor.GetCurrentUserId();
        var userRole = _httpContextAccessor.GetCurrentRole();
        var methodName = $"{nameof(GetOwnProfileHandler)}.{nameof(Handle)} UserId = {userId} =>";
        _logger.LogInformation(methodName);

        try
        {
            var currentUser = await
            (
                from user in _userManager.Users
                where !user.IsDeleted && !user.IsBlocked && user.Role == userRole && user.Id == userId
                join counterPart in _unitOfWork.CounterParts.GetAll()
                    on user.Id equals counterPart.Id into counterPartJoin
                from counterPart in counterPartJoin.DefaultIfEmpty()
                join player in _unitOfWork.Players.GetAll()
                    on user.Id equals player.Id into playerJoin
                from player in playerJoin.DefaultIfEmpty()
                let isCounterPart = user.Role == Constants.COUNTERPART
                let isPlayer = user.Role == Constants.PLAYER
                select new UserFullProfileDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    AvatarUrl = user.AvatarUrl ?? Common.Constants.DefaultAvatarUrl,
                    Role = user.Role,
                    CreatedDate = user.CreatedDate,
                    ModifiedDate = user.ModifiedDate,
                    ProfileLinked = user.ProfileLinked,
                    IsBlocked = user.IsBlocked,
                    BlockedDate = user.BlockedDate,
                    // For counterpart
                    Field = isCounterPart ? counterPart.Field : null,
                    Addresses = isCounterPart ? counterPart.Addresses : null,
                    // For player
                    BirthDate = isPlayer ? player.BirthDate : null,
                    Gender = isPlayer ? player.Gender : null,
                    FacebookUrl = isPlayer ? player.FacebookUrl : null
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
            
            if (currentUser is null)
            {
                response.ToNotFoundResponse();
                return response;
            }
            
            response.ToSuccessResponse(currentUser);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}