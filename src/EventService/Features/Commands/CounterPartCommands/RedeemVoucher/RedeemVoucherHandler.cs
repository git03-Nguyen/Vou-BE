using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.RedeemVoucher;

public class RedeemVoucherHandler : IRequestHandler<RedeemVoucherCommand, BaseResponse<UseVoucherDto>>
{
    private readonly ILogger<RedeemVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;

    public RedeemVoucherHandler(ILogger<RedeemVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<UseVoucherDto>> Handle(RedeemVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(RedeemVoucherHandler)}.{nameof(Handle)} UserId = {userId}, VoucherToPlayerId = {request.VoucherToPlayerId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<UseVoucherDto>();

        try
        {
            var now = DateTime.UtcNow;
            var voucherToPlayer = await GetVoucherUsageStateAsync(request.VoucherToPlayerId, cancellationToken);

            if (voucherToPlayer is null)
            {
                response.ToNotFoundResponse("Voucher redemption record not found");
                return response;
            }

            if (voucherToPlayer.VoucherCounterPartId != userId)
            {
                response.ToForbiddenResponse("You are not allowed to redeem this voucher");
                return response;
            }

            if (voucherToPlayer.UsedDate is not null)
            {
                response.ToBadRequestResponse("Voucher has already been redeemed");
                return response;
            }

            if (voucherToPlayer.ExpiredDate < now)
            {
                response.ToBadRequestResponse("Voucher has expired");
                return response;
            }

            var affectedRows = await _unitOfWork.VoucherToPlayers
                .Where(v => v.Id == request.VoucherToPlayerId
                            && !v.IsDeleted
                            && v.UsedDate == null
                            && v.ExpiredDate >= now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(v => v.UsedDate, now)
                    .SetProperty(v => v.UsedBy, userId)
                    .SetProperty(v => v.ModifiedDate, now), cancellationToken);

            if (affectedRows == 0)
            {
                var latestVoucherState = await GetVoucherUsageStateAsync(request.VoucherToPlayerId, cancellationToken);

                if (latestVoucherState is null)
                {
                    response.ToNotFoundResponse("Voucher redemption record not found");
                    return response;
                }

                if (latestVoucherState.VoucherCounterPartId != userId)
                {
                    response.ToForbiddenResponse("You are not allowed to redeem this voucher");
                    return response;
                }

                if (latestVoucherState.UsedDate is not null)
                {
                    response.ToBadRequestResponse("Voucher has already been redeemed");
                    return response;
                }

                if (latestVoucherState.ExpiredDate < now)
                {
                    response.ToBadRequestResponse("Voucher has expired");
                    return response;
                }

                response.ToBadRequestResponse("Voucher has already been redeemed");
                return response;
            }

            response.ToSuccessResponse(new UseVoucherDto
            {
                Id = voucherToPlayer.Id,
                VoucherId = voucherToPlayer.VoucherId,
                PlayerId = voucherToPlayer.PlayerId,
                UsedBy = userId,
                UsedDate = now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }

    private Task<VoucherRedemptionState?> GetVoucherUsageStateAsync(string voucherToPlayerId, CancellationToken cancellationToken)
    {
        return (
                from ownedVoucher in _unitOfWork.VoucherToPlayers.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on ownedVoucher.VoucherId equals voucher.Id
                where ownedVoucher.Id == voucherToPlayerId
                      && !ownedVoucher.IsDeleted
                      && !voucher.IsDeleted
                select new VoucherRedemptionState
                {
                    Id = ownedVoucher.Id,
                    VoucherId = ownedVoucher.VoucherId,
                    PlayerId = ownedVoucher.PlayerId,
                    ExpiredDate = ownedVoucher.ExpiredDate,
                    UsedDate = ownedVoucher.UsedDate,
                    VoucherCounterPartId = voucher.CounterPartId
                })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private sealed class VoucherRedemptionState
    {
        public string Id { get; init; }
        public string VoucherId { get; init; }
        public string PlayerId { get; init; }
        public DateTime ExpiredDate { get; init; }
        public DateTime? UsedDate { get; init; }
        public string? VoucherCounterPartId { get; init; }
    }
}
