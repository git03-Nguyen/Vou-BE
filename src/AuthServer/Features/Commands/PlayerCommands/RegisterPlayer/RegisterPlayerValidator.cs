using AuthServer.Common;
using FluentValidation;
using Shared.Common;

namespace AuthServer.Features.Commands.PlayerCommands.RegisterPlayer;

public class RegisterPlayerValidator : AbstractValidator<RegisterPlayerCommand>
{
    public RegisterPlayerValidator()
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("UserName is required")
            .Matches(Regexes.VALID_USERNAME)
            .WithMessage("UserName is invalid");
        
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email is required")
            .Matches(Regexes.VALID_EMAIL)
            .WithMessage("Email is invalid");
        
        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("PhoneNumber is required")
            .Matches(Regexes.VALID_PHONE)
            .WithMessage("PhoneNumber is invalid");
        
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password is required")
            .Matches(Regexes.VALID_PASSWORD)
            .WithMessage("Password is invalid");
        
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("FullName is required")
            .Matches(Regexes.VALID_FULLNAME)
            .WithMessage("FullName is invalid");

        RuleFor(x => x.AvatarUrl)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_URL)
            .WithMessage("AvatarUrl is invalid");
        
        RuleFor(x => x.BirthDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("BirthDate is required")
            .Must(x => x <= DateTime.Now)
            .WithMessage("BirthDate is invalid");

        RuleFor(x => x.Gender)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("BirthDate is required");
            
        RuleFor(x => x.FacebookUrl)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_FACEBOOK_URL)
            .WithMessage("FacebookUrl is invalid");
    }
}