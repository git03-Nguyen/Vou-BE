using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Response;

namespace AuthServer.Features.Queries.AdminQueries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, BaseResponse<GetAllUsersResponse>>
{
    private readonly ILogger<GetAllUsersHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public GetAllUsersHandler(ILogger<GetAllUsersHandler> logger, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GetAllUsersResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<GetAllUsersResponse>();
        const string methodName = $"{nameof(GetAllUsersHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var users = await
                (
                    from user in _userManager.Users
                    where !user.IsDeleted && !user.IsBlocked
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
                        Name = isCounterPart ? counterPart.Name : null,
                        Field = isCounterPart ? counterPart.Field : null,
                        Addresses = isCounterPart ? counterPart.Addresses : null,
                        // For player
                        BirthDate = isPlayer ? player.BirthDate : null,
                        Gender = isPlayer ? player.Gender : null,
                        FacebookUrl = isPlayer ? player.FacebookUrl : null
                    }
                ).AsNoTracking()
                .ToListAsync(cancellationToken);

            var responseData = new GetAllUsersResponse { Users = users };
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