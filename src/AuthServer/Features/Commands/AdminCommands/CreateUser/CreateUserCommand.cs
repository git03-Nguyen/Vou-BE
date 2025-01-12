using AuthServer.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace AuthServer.Features.Commands.AdminCommands.CreateUser;

public class CreateUserCommand : IRequest<BaseResponse<UserFullProfileDto>>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string? AvatarUrl { get; set; }
    
    public string Role { get; set; }
    
    // For CounterPart
    public string? Field { get; set; }
    public string? Address { get; set; }
    
    // For Player
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? FacebookUrl { get; set; }
}