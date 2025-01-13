using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetOwnVouchers;

public class GetOwnVouchersQuery : IRequest<BaseResponse<GetOwnVouchersResponse>>
{
}

public class GetOwnVouchersResponse
{
    public List<OwnVoucherDto> Vouchers { get; set; }
}