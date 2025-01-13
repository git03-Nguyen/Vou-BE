using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using AuthServer.Services.PubSubService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.EventMessages;
using Shared.Response;
using Shared.Services.HttpContextAccessor;
using Constants = Shared.Common.Constants;

namespace AuthServer.Features.Commands.AdminCommands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<UpdateUserHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublishService _eventPublishService;
    public UpdateUserHandler(ILogger<UpdateUserHandler> logger, UserManager<User> userManager, ICustomHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
        _eventPublishService = eventPublishService;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        var methodName = string.Empty;

        try
        {
            var adminId = _httpContextAccessor.GetCurrentUserId();
            methodName = $"{nameof(UpdateUserHandler)}.{nameof(Handle)} Admin = {adminId}, Request = {JsonSerializer.Serialize(request)} =>";
            _logger.LogInformation(methodName);
            
            // 1. Check user exists
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user is null || user.IsDeleted)
            {
                response.ToNotFoundResponse();
                return response;
            }
            _logger.LogInformation($"Request..........{JsonSerializer.Serialize(request)}");
            //2. Update user
            Player? player = null;
            CounterPart? counterpart=null;
             var userRole=_userManager.GetRolesAsync(user).Result;
             if (userRole.Contains(Constants.PLAYER))
             {
                player = await _unitOfWork.Players
                    .Where(p => p.Id == user.Id&& !p.IsDeleted) 
                .FirstOrDefaultAsync(cancellationToken);
                if (player is null)
                {
                    response.ToNotFoundResponse();
                    return response;
                }
                else
                {
                    if (request.BirthDate.HasValue)
                    {
                        player.BirthDate = request.BirthDate.Value;
                    }

                    if (request.Gender.HasValue)
                    {
                        player.Gender = request.Gender.Value;
                        _logger.LogInformation(player.Gender.ToString());
                    }

                    if (request.FacebookUrl != null)
                    {
                        player.FacebookUrl = request.FacebookUrl;
                    }
                }
             }  
             else if (userRole.Contains(Constants.COUNTERPART))
             {
                 counterpart = await _unitOfWork.CounterParts
                     .Where(c => c.Id == user.Id && !c.IsDeleted)
                     .FirstOrDefaultAsync(cancellationToken);
                 if (counterpart is null)
                 {
                     response.ToNotFoundResponse();
                     return response;
                 }
                 else
                 {
                     if (request.Field != null)
                     {
                         counterpart.Field = request.Field;
                     }

                     if (request.Addresses != null)
                     {
                         counterpart.Addresses = request.Addresses;
                     }
                 }
             }
             
             if (request.FullName != null)
             {
                     user.FullName = request.FullName;
             }
             if (request.AvatarUrl != null)
             {
                     user.AvatarUrl = request.AvatarUrl;
             }
             if (request.ProfileLinked != null)
             {
                        user.ProfileLinked = request.ProfileLinked;
             }

             if (player != null)
             {
                 _unitOfWork.Players.Update(player);
             }
             if (counterpart != null)
             {
               _unitOfWork.CounterParts.Update(counterpart);
             }
             await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseData=new UserFullProfileDto
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
                Field =counterpart?.Field,
                Addresses = counterpart?.Addresses,
                // For player
                BirthDate = player?.BirthDate,
                Gender = player?.Gender,
                FacebookUrl = player?.FacebookUrl 
            };
            
            // Publish event
            await PublishMessageAsync(responseData, cancellationToken);
            
            return response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
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