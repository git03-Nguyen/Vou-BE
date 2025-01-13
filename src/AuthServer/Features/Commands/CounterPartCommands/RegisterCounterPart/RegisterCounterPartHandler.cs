using System.Text.Json;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using AuthServer.Services.EmailService;
using AuthServer.Services.PubSubService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Contracts.EventMessages;
using Shared.Response;

namespace AuthServer.Features.Commands.CounterPartCommands.RegisterCounterPart;

public class RegisterCounterPartHandler : IRequestHandler<RegisterCounterPartCommand, BaseResponse<UserFullProfileDto>>
{
    private readonly ILogger<RegisterCounterPartValidator> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IEventPublishService _eventPublishService;
    public RegisterCounterPartHandler(ILogger<RegisterCounterPartValidator> logger, UserManager<User> userManager, IUnitOfWork unitOfWork, IEmailService emailService, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _eventPublishService = eventPublishService;
    }

    public async Task<BaseResponse<UserFullProfileDto>> Handle(RegisterCounterPartCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserFullProfileDto>();
        User? backupUser = null;
        var methodName = $"{nameof(RegisterCounterPartHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
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
            
            //Generate OTP activate code
            var random = new Random();
            var otp =  random.Next(100000, 999999).ToString(); 
            
            // 2. Create user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = Constants.COUNTERPART,
                OtpActivateCode = otp,
                OtpActivateExpiredTime = DateTime.Now.AddMinutes(5),
                EmailConfirmed = true
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
            
            // 4. Add to CounterPart
            var counterPart = await AddToCounterPart(request, user, cancellationToken);
            var responseData = new UserFullProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl ?? Common.Constants.DefaultAvatarUrl,
                Role = user.Role,
                Field = counterPart.Field,
                Addresses = counterPart.Addresses
            };
            //Save to database
            await _userManager.UpdateAsync(user);
            
            //5. Send OTP to activate account
           // await SendActivateOtp(user);
           
            
            // 6. Publish message to PubSub
            // await PublishMessageAsync(responseData, cancellationToken);
            
            response.ToSuccessResponse(responseData, "User created successfully. Please check your email to activate your account.");
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
    
    private async Task<CounterPart> AddToCounterPart(RegisterCounterPartCommand request, User user, CancellationToken cancellationToken)
    {
        var counterPart = new CounterPart
        {
            Id = user.Id,
            Field = request.Field,
            Addresses = request.Address,
        };
        await _unitOfWork.CounterParts.AddAsync(counterPart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return counterPart;
    }
    
    //send OTP to activate account
    private async Task SendActivateOtp(User user)
    {
        if (user.OtpActivateCode != null)
            await _emailService.SendEmailAsync(user.Email, Common.Constants.ActivateSubjectEmail,
                Common.Constants.GetOtpActivateAccountMessage(user.OtpActivateCode));
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