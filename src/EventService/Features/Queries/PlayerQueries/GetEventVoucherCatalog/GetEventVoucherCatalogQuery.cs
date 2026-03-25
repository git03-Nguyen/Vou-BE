using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetEventVoucherCatalog;

public class GetEventVoucherCatalogQuery : IRequest<BaseResponse<GetEventVoucherCatalogResponse>>
{
    public string EventId { get; set; } = string.Empty;
}

public class GetEventVoucherCatalogResponse
{
    public string EventId { get; set; } = string.Empty;
    public string? ShakeVoucherId { get; set; }
    public long? ShakePrice { get; set; }
    public List<EventVoucherCatalogItemDto> Vouchers { get; set; } = new();
}

public class EventVoucherCatalogItemDto
{
    public bool IsShakeVoucher { get; set; }
    public int PlayerOwnedCount { get; set; }
    public int PlayerAvailableCount { get; set; }
    public VoucherDto Voucher { get; set; } = new();
}
