using AuthServer.Data.Models;
using AuthServer.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace AuthServer.Features.Commands.UserCommands.UpdateOwnUserProfile;

public class UpdateOwnUserProfileCommand : IRequest<BaseResponse<UserFullProfileDto>>
{
 
    // for general account
    public string? AvatarUrl { get; set; }
    public ProfileLinked? ProfileLinked { get; set; }

    public string? FullName { get; set; }
    
    //for player
    public DateTime? BirthDate { get; set; }
    
    public Gender? Gender { get; set; }
    
    public string? FacebookUrl { get; set; }
    
    //for counterpart
    public string? Field { get; set; }
    
    public string? Addresses { get; set; }
}