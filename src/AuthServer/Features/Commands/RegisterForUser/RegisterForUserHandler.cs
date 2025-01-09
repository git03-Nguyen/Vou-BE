using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace AuthServer.Features.Commands.RegisterForUser;

public class RegisterForUserHandler : IRequestHandler<RegisterForUserCommand, BaseResponse<UserShortDto>>
{
    private readonly ILogger<RegisterForUserHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterForUserHandler(ILogger<RegisterForUserHandler> logger, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<UserShortDto>> Handle(RegisterForUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserShortDto>();
        var methodName = $"{nameof(RegisterForUserHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        User? backupUser = null;

        try
        {
            // 1. Check if exists
            var email = request.Email.Trim();
            var userName = request.UserName.Trim();
            var phoneNumber = request.PhoneNumber.Trim();
            var existedUser = await _userManager.Users
                .Where(u => 
                    !u.IsDeleted 
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
                Role = request.Role,
            };
            
            // 3. Add to table User
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
            
            // Add to table CounterPart or Player
            if (request.Role == Constants.COUNTERPART)
            {
                await AddToCounterPart(user);
            }
            else if (request.Role == Constants.PLAYER)
            {
                await AddToPlayer(user);
            }
            
            var responseData = new UserShortDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl ?? Constants.DefaultAvatarUrl,
                Role = user.Role
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
    
    private async Task AddToCounterPart(User user)
    {
        var counterPart = new CounterPart
        {
            Id = Guid.NewGuid().ToString(),
            Name = 
        };
        await _unitOfWork.CounterPartRepository.AddAsync(counterPart);
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task AddToPlayer(User user)
    {
        var player = new Player
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl ?? Constants.DefaultAvatarUrl,
            Role = user.Role
        };
        await _unitOfWork.PlayerRepository.AddAsync(player);
        await _unitOfWork.SaveChangesAsync();
    }
}