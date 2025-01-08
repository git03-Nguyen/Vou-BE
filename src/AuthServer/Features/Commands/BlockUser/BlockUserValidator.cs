using AuthServer.Common;
using FluentValidation;

namespace AuthServer.Features.Commands.BlockUser;

public class BlockUserValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserValidator()
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