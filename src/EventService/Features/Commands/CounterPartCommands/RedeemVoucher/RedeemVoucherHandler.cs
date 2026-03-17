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
            var voucherToPlayer = await (
                    from ownedVoucher in _unitOfWork.VoucherToPlayers.GetAll()
                    join voucher in _unitOfWork.Vouchers.GetAll()
                        on ownedVoucher.VoucherId equals voucher.Id
                    where ownedVoucher.Id == request.VoucherToPlayerId
                          && !ownedVoucher.IsDeleted
                          && !voucher.IsDeleted
                    select new { OwnedVoucher = ownedVoucher, Voucher = voucher })
                .FirstOrDefaultAsync(cancellationToken);

            if (voucherToPlayer is null)
            {
                response.ToNotFoundResponse("Voucher redemption record not found");
                return response;
            }

            if (voucherToPlayer.Voucher.CounterPartId != userId)
            {
                response.ToForbiddenResponse("You are not allowed to redeem this voucher");
                return response;
            }

            if (voucherToPlayer.OwnedVoucher.UsedDate is not null)
            {
                response.ToBadRequestResponse("Voucher has already been redeemed");
                return response;
            }

            if (voucherToPlayer.OwnedVoucher.ExpiredDate < DateTime.UtcNow)
            {
                response.ToBadRequestResponse("Voucher has expired");
                return response;
            }

            voucherToPlayer.OwnedVoucher.UsedDate = DateTime.UtcNow;
            voucherToPlayer.OwnedVoucher.UsedBy = userId;
            voucherToPlayer.OwnedVoucher.ModifiedDate = DateTime.UtcNow;
            _unitOfWork.VoucherToPlayers.Update(voucherToPlayer.OwnedVoucher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            response.ToSuccessResponse(new UseVoucherDto
            {
                Id = voucherToPlayer.OwnedVoucher.Id,
                VoucherId = voucherToPlayer.OwnedVoucher.VoucherId,
                PlayerId = voucherToPlayer.OwnedVoucher.PlayerId,
                UsedBy = userId,
                UsedDate = voucherToPlayer.OwnedVoucher.UsedDate.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
