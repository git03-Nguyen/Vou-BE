using AuthServer.Common;
using FluentValidation;
using Constants = Shared.Common.Constants;

namespace AuthServer.Features.Commands.UserCommands.UserLogin;

public class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email or username is required");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password is required")
            .Length(6, 50)
            .WithMessage("Password must be between 6 and 50 characters")
            .Matches(Regexes.VALID_PASSWORD)
            .WithMessage("Password must contain uppercase letters, lowercase letters, numbers and special characters");
        
        When(x => x.EmailOrUserName.Contains('@'), () =>
        {
            RuleFor(x => x.EmailOrUserName)
                .Cascade(CascadeMode.Stop)
                .Matches(Regexes.VALID_EMAIL)
                .WithMessage("Email is not valid");
        });
        
        RuleFor(x => x.Role) 
            .NotNull()
            .NotNull()
            .Must(x => x == Constants.PLAYER || x == Constants.ADMIN || x == Constants.COUNTERPART)
            .WithMessage("Role is not valid");
    }
}