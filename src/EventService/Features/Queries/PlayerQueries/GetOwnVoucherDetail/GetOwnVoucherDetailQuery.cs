using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetOwnVoucherDetail;

public class GetOwnVoucherDetailQuery : IRequest<BaseResponse<PlayerVoucherDetailDto>>
{
    public string VoucherToPlayerId { get; set; } = string.Empty;
}

public class PlayerVoucherDetailDto
{
    public string VoucherToPlayerId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public DateTime? AcquiredDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public DateTime? UsedDate { get; set; }
    public string? UsedBy { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsExpired { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRedeemedByCounterPart { get; set; }
    public string Status { get; set; } = string.Empty;
    public VoucherDto Voucher { get; set; } = new();
}
