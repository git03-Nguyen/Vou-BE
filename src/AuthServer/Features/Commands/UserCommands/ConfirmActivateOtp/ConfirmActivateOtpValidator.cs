using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.UserCommands.ConfirmActivateOtp;

public class ConfirmActivateOtpValidator : AbstractValidator<ConfirmActivateOtpCommand>
{
    public ConfirmActivateOtpValidator()
    {
        RuleFor(x => x.Otp)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Otp is required")
            .Length(6)
            .WithMessage("Otp is 6 digits");
        
        RuleFor(x=>x.UserNameOrEmail)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("UserNameOrEmail is required");
    }
}