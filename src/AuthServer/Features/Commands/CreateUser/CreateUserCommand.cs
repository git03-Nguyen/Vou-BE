using AuthServer.Common;
using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.CreateUser;

public class CreateUserCommand : IRequest<BaseResponse<UserShortDto>>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; } = Constants.PLAYER;
    // public IFormFile? AvatarImage { get; set; }
}