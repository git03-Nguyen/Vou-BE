using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.UserCommands.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Old password is required")
            .Matches(Regexes.VALID_PASSWORD)
            .WithMessage("Old password is invalid");
        
        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("New password is required")
            .Matches(Regexes.VALID_PASSWORD)
            .WithMessage("New password is invalid");
        
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .Must(x => !string.Equals(x.OldPassword, x.NewPassword, StringComparison.Ordinal))
            .WithMessage("New password must be different from old password");
    }
}