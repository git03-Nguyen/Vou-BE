using FluentValidation;

namespace EventService.Features.Queries.AdminQueries.GetAllEvents;

public class GetAllEventsValidator : AbstractValidator<GetAllEventsQuery>
{
    public GetAllEventsValidator()
    {
        RuleFor(x => x.CounterPartId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.CounterPartId))
            .WithMessage("CounterPartId is too long");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Search is too long");
    }
}
