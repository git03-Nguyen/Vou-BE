using AuthServer.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace AuthServer.Features.Commands.CounterPartCommands.RegisterCounterPart;

public class RegisterCounterPartCommand : IRequest<BaseResponse<UserFullProfileDto>>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    // public IFormFile? AvatarImage { get; set; }
    public string Field { get; set; }
    public string Addresses { get; set; }
}