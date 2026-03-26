using FluentValidation;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnEvent;

public class GetOwnEventValidator : AbstractValidator<GetOwnEventQuery>
{
    public GetOwnEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}
