using FluentValidation;

namespace EventService.Features.Queries.PlayerQueries.GetEventDetail;

public class GetEventDetailValidator : AbstractValidator<GetEventDetailQuery>
{
    public GetEventDetailValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}
