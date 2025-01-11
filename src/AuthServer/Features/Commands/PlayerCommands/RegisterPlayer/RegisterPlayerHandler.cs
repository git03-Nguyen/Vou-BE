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

namespace AuthServer.Features.Commands.PlayerCommands.RegisterPlayer;

public class RegisterPlayerHandler : IRequestHandler<RegisterPlayerCommand, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<RegisterPlayerValidator> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterPlayerHandler(ILogger<RegisterPlayerValidator> logger, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(RegisterPlayerCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        User? backupUser = null;
        var methodName = $"{nameof(RegisterPlayerHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
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
                Role = Constants.COUNTERPART
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
            var resultRole = await _userManager.AddToRoleAsync(user, user.Role);
            if (!resultRole.Succeeded)
            {
                _logger.LogError($"{methodName} Failed to add role to user: {JsonSerializer.Serialize(resultRole.Errors)}");
                response.ToBadRequestResponse("Failed to add role to user");
                await RollbackUserCreation(backupUser);
                return response;
            }
            
            // 4. Add to Player
            var player = await AddToPlayer(request, user, cancellationToken);
            var responseData = new UserFullProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl ?? Common.Constants.DefaultAvatarUrl,
                Role = user.Role,
                Gender = player.Gender,
                FacebookUrl = player.FacebookUrl,
                BirthDate = player.BirthDate
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
    
    private async Task<Player> AddToPlayer(RegisterPlayerCommand request, User user, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = user.Id,
            BirthDate = request.BirthDate,
            Gender = request.Gender ?? Gender.Other,
            FacebookUrl = request.FacebookUrl
        };
        await _unitOfWork.Players.AddAsync(player, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return player;
    }
}