using FluentValidation;

namespace EventService.Features.Queries.PlayerQueries.GetOwnVoucherDetail;

public class GetOwnVoucherDetailValidator : AbstractValidator<GetOwnVoucherDetailQuery>
{
    public GetOwnVoucherDetailValidator()
    {
        RuleFor(x => x.VoucherToPlayerId)
            .NotNull()
            .NotEmpty()
            .WithMessage("VoucherToPlayerId is required")
            .MaximumLength(100)
            .WithMessage("VoucherToPlayerId must not exceed 100 characters");
    }
}
