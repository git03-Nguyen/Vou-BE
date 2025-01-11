using AuthServer.DTOs;
using MediatR;
using Shared.Response;

namespace AuthServer.Features.Commands.CounterPartCommands.RegisterCounterPart;

public class RegisterCounterPartCommand : IRequest<BaseResponse<UserFullProfileDto>>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string Field { get; set; }
    public string Address { get; set; }
    public string? AvatarUrl { get; set; }
}