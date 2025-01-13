using System.Text.Json;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using AuthServer.Services.PubSubService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Contracts;
using Shared.Contracts.EventMessages;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Commands.UserCommands.UpdateOwnUserProfile;

public class UpdateOwnUserProfileHandler : IRequestHandler<UpdateOwnUserProfileCommand, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<UpdateOwnUserProfileValidator> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    private readonly IEventPublishService _eventPublishService;
    public UpdateOwnUserProfileHandler(ILogger<UpdateOwnUserProfileValidator> logger, UserManager<User> userManager, IUnitOfWork unitOfWork, ICustomHttpContextAccessor httpContextAccessor, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _eventPublishService = eventPublishService;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(UpdateOwnUserProfileCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        User? backupUser = null;
        var methodName = $"{nameof(UpdateOwnUserProfileHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        var userId = _httpContextAccessor.GetCurrentUserId();
        var userRole = _httpContextAccessor.GetCurrentRole();

        try
        {
            // 1. Check if the user exists
            var currentUser = await (from user in _userManager.Users
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

            // 2. Update user fields based on role
            if (userRole == Constants.PLAYER)
            {
                var player = await _unitOfWork.Players
                    .Where(p => p.Id == userId && !p.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (player is null)
                {
                    response.ToBadRequestResponse("Player not found");
                    return response;
                }
                await UpdatePlayerProfile(player, request);
            }
            else if (userRole == Constants.COUNTERPART)
            {
                var counterPart = await _unitOfWork.CounterParts
                    .Where(c => c.Id == userId && !c.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (counterPart is null)
                {
                    response.ToBadRequestResponse("Counterpart not found");
                    return response;
                }

                await UpdateCounterPartProfile(counterPart, request);
            }

            //General info update
            if (request.FullName is not null)
            {
                currentUser.FullName = request.FullName;
            }

            if (request.AvatarUrl is not null)
            {
                currentUser.AvatarUrl = request.AvatarUrl;
            }

            var updatedUser = new User
            {
                Id = currentUser.Id,
                FullName = currentUser.FullName,
                AvatarUrl = currentUser.AvatarUrl,
            };
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _userManager.UpdateAsync(updatedUser);

            // 3. Prepare response data
            var responseData = new UserFullProfileDto
            {
                Id = userId,
                FullName = currentUser.FullName,
                AvatarUrl = currentUser.AvatarUrl,
                BirthDate = userRole == Constants.PLAYER ? currentUser.BirthDate : null,
                Gender = userRole == Constants.PLAYER ? currentUser.Gender : null,
                FacebookUrl = userRole == Constants.PLAYER ? currentUser.FacebookUrl : null,
                Field = userRole == Constants.COUNTERPART ? currentUser.Field : null,
                Addresses = userRole == Constants.COUNTERPART ? currentUser.Addresses : null
            };
            
            await PublishMessageAsync(responseData, cancellationToken);

            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
            await RollbackUserCreation(backupUser);
        }

        return response;
}
    
    private async Task RollbackUserCreation(User? user)
    {
        if (user is not null)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    private Task UpdatePlayerProfile(Player player, UpdateOwnUserProfileCommand request)
    {

        if (request.BirthDate is not null)
        {
            player.BirthDate = request.BirthDate;
        }

        if (request.Gender is not null)
        {
            player.Gender = request.Gender.Value;
        }

        if (request.FacebookUrl is not null)
        {
            player.FacebookUrl = request.FacebookUrl;
        }

        _unitOfWork.Players.Update(player);
        return Task.CompletedTask;
    }
    
    private Task UpdateCounterPartProfile(CounterPart counterPart, UpdateOwnUserProfileCommand request)
    {
        if (request.Field is not null)
        {
            counterPart.Field = request.Field;
        }

        if (request.Addresses is not null)
        {
            counterPart.Addresses = request.Addresses;
        }

        _unitOfWork.CounterParts.Update(counterPart);
        return Task.CompletedTask;
    }
    
    // Publish message to PubSub
    private async Task PublishMessageAsync(UserFullProfileDto user, CancellationToken cancellationToken)
    {
        var message = new UserUpdatedEvent 
        {
            UserId = user.Id,
            Role = user.Role,
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            FacebookUrl = user.FacebookUrl,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Addresses = user.Addresses,
            Field = user.Field
        };
        await _eventPublishService.PublishUserUpdatedEventAsync(message, cancellationToken);
    }
}
