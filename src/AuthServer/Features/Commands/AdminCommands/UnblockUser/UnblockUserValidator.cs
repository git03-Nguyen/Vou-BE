using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.AdminCommands.UnblockUser;

public class UnblockUserValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email or username is required");

        When(x => x.EmailOrUserName.Contains('@'), () =>
        {
            RuleFor(x => x.EmailOrUserName)
                .Cascade(CascadeMode.Stop)
                .Matches(Regexes.VALID_EMAIL)
                .WithMessage("Email is not valid");
        });
    }
}