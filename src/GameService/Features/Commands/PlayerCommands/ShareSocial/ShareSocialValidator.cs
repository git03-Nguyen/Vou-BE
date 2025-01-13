using FluentValidation;

namespace GameService.Features.Commands.PlayerCommands.ShareSocial;

public class ShareSocialValidator : AbstractValidator<ShareSocialCommand>
{
    public ShareSocialValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}