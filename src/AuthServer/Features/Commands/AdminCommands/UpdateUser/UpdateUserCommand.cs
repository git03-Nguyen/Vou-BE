using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace AuthServer.Features.Commands.AdminCommands.UpdateUser;

public class UpdateUserCommand : IRequest<BaseResponse<UserFullProfileDto>>
{
    // for general account
    public required string Id { get; set; }
    public string? AvatarUrl { get; set; }
    public ProfileLinked? ProfileLinked { get; set; }

    public string? FullName { get; set; }
    
    //for player
    public DateTime? BirthDate { get; }
    
    public Gender? Gender { get; set; }
    
    public string? FacebookUrl { get; set; }
    
    //for counterpart
    public string? Field { get; set; }
    
    public string Addresses { get; set; }
    
    
}