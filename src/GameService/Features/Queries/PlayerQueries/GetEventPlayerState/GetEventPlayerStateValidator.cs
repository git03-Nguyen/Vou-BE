using FluentValidation;

namespace GameService.Features.Queries.PlayerQueries.GetEventPlayerState;

public class GetEventPlayerStateValidator : AbstractValidator<GetEventPlayerStateQuery>
{
    public GetEventPlayerStateValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}
