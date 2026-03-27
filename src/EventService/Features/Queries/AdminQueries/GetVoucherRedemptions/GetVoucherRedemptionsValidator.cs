using FluentValidation;

namespace EventService.Features.Queries.AdminQueries.GetVoucherRedemptions;

public class GetVoucherRedemptionsValidator : AbstractValidator<GetVoucherRedemptionsQuery>
{
    private static readonly string[] AllowedStatuses = ["available", "used", "redeemed", "expired"];

    public GetVoucherRedemptionsValidator()
    {
        RuleFor(x => x.EventId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.EventId))
            .WithMessage("EventId is too long");

        RuleFor(x => x.VoucherId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.VoucherId))
            .WithMessage("VoucherId is too long");

        RuleFor(x => x.CounterPartId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.CounterPartId))
            .WithMessage("CounterPartId is too long");

        RuleFor(x => x.PlayerId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.PlayerId))
            .WithMessage("PlayerId is too long");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Search is too long");

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || AllowedStatuses.Contains(status.Trim().ToLowerInvariant()))
            .WithMessage("Status must be one of: available, used, redeemed, expired");
    }
}
