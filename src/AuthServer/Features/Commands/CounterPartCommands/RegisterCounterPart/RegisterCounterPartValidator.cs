using AuthServer.Common;
using FluentValidation;
using Shared.Common;

namespace AuthServer.Features.Commands.CounterPartCommands.RegisterCounterPart;

public class RegisterCounterPartValidator : AbstractValidator<RegisterCounterPartCommand>
{
    public RegisterCounterPartValidator()
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
        
        RuleFor(x => x.AvatarUrl)
            .Cascade(CascadeMode.Stop)
            .Matches(Regexes.VALID_URL)
            .WithMessage("AvatarUrl is invalid");
        
        RuleFor(x => x.Field)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Field is required")
            .Matches(Regexes.VALID_COUNTERPART_FIELD)
            .WithMessage("Field is invalid");

        RuleFor(x => x.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Addresses is required");
        
    }
}