using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.RegisterForPlayer;

public class RegisterForPlayerValidator : AbstractValidator<RegisterForPlayerCommand>
{
    public RegisterForPlayerValidator()
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
            .NotNull()
            .NotEmpty()
            .WithMessage("FullName is required")
            .Matches(Regexes.VALID_FULLNAME)
            .WithMessage("FullName is invalid");

        When(x => x.AvatarImage is not null, () =>
        {
            RuleFor(x => x.AvatarImage!.Length)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(1024 * 1024)
                .WithMessage("AvatarImage is too large");
        });
    }
}