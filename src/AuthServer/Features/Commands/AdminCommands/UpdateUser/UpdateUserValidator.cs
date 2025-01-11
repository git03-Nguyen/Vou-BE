using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.AdminCommands.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
    }
}