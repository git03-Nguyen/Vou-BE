using System.Text.Json.Serialization;
using AuthServer.DTOs;
using MediatR;
using Shared.Common;
using Shared.Response;

namespace AuthServer.Features.Commands.UserCommands.UserLogin;

public class UserLoginCommand : IRequest<BaseResponse<UserLoginResponseDto>>
{
    public string EmailOrUserName { get; set; }
    public string Password { get; set; }
    
    [JsonIgnore]
    internal string Role { get; set; } = Constants.PLAYER;
}