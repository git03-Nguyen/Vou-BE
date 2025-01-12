using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnVouchers;

public class GetOwnVouchersQuery : IRequest<BaseResponse<GetOwnVouchersResponse>>
{
}

public class GetOwnVouchersResponse
{
    public List<VoucherDto> Vouchers { get; set; }
}