using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.UserCommands.UpdateOwnUserProfile;

public class UpdateOwnUserProfileValidator : AbstractValidator<UpdateOwnUserProfileCommand>
{
    public UpdateOwnUserProfileValidator()
    {
      
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_FULLNAME)
            .WithMessage("FullName is invalid");

        RuleFor(x => x.AvatarUrl)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_URL)
            .WithMessage("AvatarUrl is invalid");
        
        /*RuleFor(x => x.BirthDate)
            .Cascade(CascadeMode.Stop)
            .Must(x => x <= DateTime.Now)
            .WithMessage("BirthDate is invalid");*/

        RuleFor(x => x.Gender)
            .Cascade(CascadeMode.Stop)
            .IsInEnum()
            .WithMessage("Gender is not valid");
            
        RuleFor(x => x.FacebookUrl)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_FACEBOOK_URL)
            .WithMessage("FacebookUrl is invalid");
    }
}