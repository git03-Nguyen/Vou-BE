using FluentValidation;

namespace EventService.Features.Queries.PlayerQueries.GetEventVoucherCatalog;

public class GetEventVoucherCatalogValidator : AbstractValidator<GetEventVoucherCatalogQuery>
{
    public GetEventVoucherCatalogValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId is too long");
    }
}
