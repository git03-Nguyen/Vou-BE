using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetOwnVouchers;

public class GetOwnVouchersHandler : IRequestHandler<GetOwnVouchersQuery, BaseResponse<GetOwnVouchersResponse>>
{
    private readonly ILogger<GetOwnVouchersHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetOwnVouchersHandler(ILogger<GetOwnVouchersHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetOwnVouchersResponse>> Handle(GetOwnVouchersQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnVouchersHandler)}.{nameof(Handle)} UserId = {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetOwnVouchersResponse>();

        try
        {
            var vouchers = await
            (
                from player in _unitOfWork.Players.GetAll()
                join voucherToPlayer in _unitOfWork.VoucherToPlayers.GetAll()
                    on player.Id equals voucherToPlayer.PlayerId
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on voucherToPlayer.VoucherId equals voucher.Id
                where !voucher.IsDeleted
                    && player.Id == userId
                    && (voucherToPlayer.ExpiredDate >= DateTime.Now
                        || voucherToPlayer.UsedDate != null)
                group voucher by new { voucherToPlayer.EventId, voucher.Id } into g
                select new OwnVoucherDto
                {
                    EventId = g.Key.EventId,
                    Count = g.Count(),
                    Voucher = new VoucherDto
                    {
                        Id = g.Key.Id,
                        Title = g.First().Title,
                        Value = g.First().Value,
                        ImageUrl = g.First().ImageUrl
                    }
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new GetOwnVouchersResponse { Vouchers = vouchers };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}