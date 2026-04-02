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
    public DateTime? AcquiredFrom { get; set; }
    public DateTime? AcquiredTo { get; set; }
    public DateTime? UsedFrom { get; set; }
    public DateTime? UsedTo { get; set; }
    public DateTime? ExpiredFrom { get; set; }
    public DateTime? ExpiredTo { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

public class GetVoucherRedemptionsResponse
{
    public int TotalCount { get; set; }
    public int AvailableCount { get; set; }
    public int UsedCount { get; set; }
    public int RedeemedCount { get; set; }
    public int ExpiredCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<AdminVoucherRedemptionDto> VoucherRedemptions { get; set; } = new();
}