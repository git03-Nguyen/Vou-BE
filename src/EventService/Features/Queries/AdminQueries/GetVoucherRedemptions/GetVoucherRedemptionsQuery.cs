using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.AdminQueries.GetVoucherRedemptions;

public class GetVoucherRedemptionsQuery : IRequest<BaseResponse<GetVoucherRedemptionsResponse>>
{
    public string? EventId { get; set; }
    public string? VoucherId { get; set; }
    public string? CounterPartId { get; set; }
    public string? PlayerId { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
}

public class GetVoucherRedemptionsResponse
{
    public int TotalCount { get; set; }
    public int AvailableCount { get; set; }
    public int UsedCount { get; set; }
    public int RedeemedCount { get; set; }
    public int ExpiredCount { get; set; }
    public List<AdminVoucherRedemptionDto> VoucherRedemptions { get; set; } = new();
}
