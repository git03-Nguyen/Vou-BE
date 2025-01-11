using System.Text.Json;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Contracts;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace AuthServer.Features.Commands.AdminCommands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<CreateUserValidator> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _httpContextAccessor;
    public CreateUserHandler(ILogger<CreateUserValidator> logger, UserManager<User> userManager, IUnitOfWork unitOfWork, ICustomHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        User? backupUser = null;
        var methodName = string.Empty;

        try
        {
            var adminId = _httpContextAccessor.GetCurrentUserId();
            methodName = $"{nameof(CreateUserHandler)}.{nameof(Handle)} Admin: {adminId}, Request = {JsonSerializer.Serialize(request)} =>";
            _logger.LogInformation(methodName);
            
            // 1. Check if exists
            var email = request.Email.Trim();
            var userName = request.UserName.Trim();
            var phoneNumber = request.PhoneNumber.Trim();
            var existedUser = await _userManager.Users
                .Where(u => 
                    !u.IsDeleted && !u.IsBlocked
                    && (u.NormalizedEmail == email.ToUpper()
                        || u.NormalizedUserName == userName.ToUpper() 
                        || u.PhoneNumber == phoneNumber))
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
                Role = request.Role
            };
            
            // 3. Add user
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"{methodName} Failed to create user: {JsonSerializer.Serialize(result.Errors)}");
                response.ToBadRequestResponse("Failed to create user");
                return response;
            }

            backupUser = user;
            var resultRole = await _userManager.AddToRoleAsync(user, request.Role);
            if (!resultRole.Succeeded)
            {
                _logger.LogError($"{methodName} Failed to add role to user: {JsonSerializer.Serialize(resultRole.Errors)}");
                response.ToBadRequestResponse("Failed to add role to user");
                await RollbackUserCreation(backupUser);
                return response;
            }
            
            // 4. Add to Player or CounterPart
            Player? player = null;
            CounterPart? counterPart = null;
            if (user.Role == Constants.ADMIN)
            {
                // do nothing
            }
            else if (user.Role == Constants.COUNTERPART)
            {
                counterPart = await AddToCounterPart(request, user, adminId, cancellationToken);
            }
            else if (user.Role == Constants.PLAYER)
            {
                player = await AddToPlayer(request, user, adminId, cancellationToken);
            }
            else
            {
                await RollbackUserCreation(backupUser);
                _logger.LogError($"{methodName} Cannot create user with role {user.Role}");
                response.ToBadRequestResponse($"Cannot create user with role {user.Role}");
            }
            
            var responseData = new UserFullProfileDto
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
                Field = counterPart?.Field,
                Addresses = counterPart?.Addresses,
                // For player
                BirthDate = player?.BirthDate,
                Gender = player?.Gender,
                FacebookUrl = player?.FacebookUrl
            };
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
    
    private async Task<CounterPart> AddToCounterPart(CreateUserCommand request, User user, string adminId, CancellationToken cancellationToken)
    {
        var counterPart = new CounterPart
        {
            Id = user.Id,
            Field = request.Field ?? string.Empty,
            Addresses = request.Address,
            CreatedBy = adminId
        };
        await _unitOfWork.CounterParts.AddAsync(counterPart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return counterPart;
    }
    
    private async Task<Player> AddToPlayer(CreateUserCommand request, User user, string adminId, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = user.Id,
            BirthDate = request.BirthDate,
            Gender = request.Gender ?? Gender.Other,
            FacebookUrl = request.FacebookUrl,
            CreatedBy = adminId
        };
        await _unitOfWork.Players.AddAsync(player, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return player;
    }
}