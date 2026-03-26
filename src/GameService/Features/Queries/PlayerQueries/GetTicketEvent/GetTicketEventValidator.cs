using FluentValidation;

namespace GameService.Features.Queries.PlayerQueries.GetTicketEvent;

public class GetTicketEventValidator : AbstractValidator<GetTicketEventQuery>
{
    public GetTicketEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}
