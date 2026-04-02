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

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .When(x => x.PageNumber.HasValue)
            .WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .When(x => x.PageSize.HasValue)
            .WithMessage("PageSize must be between 1 and 200");

        RuleFor(x => x)
            .Must(x => !x.AcquiredFrom.HasValue || !x.AcquiredTo.HasValue || x.AcquiredFrom.Value <= x.AcquiredTo.Value)
            .WithMessage("AcquiredFrom must be earlier than or equal to AcquiredTo");

        RuleFor(x => x)
            .Must(x => !x.UsedFrom.HasValue || !x.UsedTo.HasValue || x.UsedFrom.Value <= x.UsedTo.Value)
            .WithMessage("UsedFrom must be earlier than or equal to UsedTo");

        RuleFor(x => x)
            .Must(x => !x.ExpiredFrom.HasValue || !x.ExpiredTo.HasValue || x.ExpiredFrom.Value <= x.ExpiredTo.Value)
            .WithMessage("ExpiredFrom must be earlier than or equal to ExpiredTo");
    }
}