using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetOwnVoucherDetail;

public class GetOwnVoucherDetailHandler : IRequestHandler<GetOwnVoucherDetailQuery, BaseResponse<PlayerVoucherDetailDto>>
{
    private readonly ILogger<GetOwnVoucherDetailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;

    public GetOwnVoucherDetailHandler(
        ILogger<GetOwnVoucherDetailHandler> logger,
        IUnitOfWork unitOfWork,
        ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<PlayerVoucherDetailDto>> Handle(GetOwnVoucherDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnVoucherDetailHandler)}.{nameof(Handle)} UserId = {userId}, VoucherToPlayerId = {request.VoucherToPlayerId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<PlayerVoucherDetailDto>();

        try
        {
            var now = DateTime.UtcNow;
            var voucherDetail = await (
                from voucherToPlayer in _unitOfWork.VoucherToPlayers.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on voucherToPlayer.VoucherId equals voucher.Id
                where !voucherToPlayer.IsDeleted
                      && !voucher.IsDeleted
                      && voucherToPlayer.Id == request.VoucherToPlayerId
                      && voucherToPlayer.PlayerId == userId
                select new
                {
                    VoucherToPlayer = voucherToPlayer,
                    Voucher = voucher,
                    IssuedQuantity = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id),
                    UsedQuantity = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id && vp.UsedDate != null)
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

            if (voucherDetail is null)
            {
                response.ToNotFoundResponse("Voucher not found");
                return response;
            }

            var isUsed = voucherDetail.VoucherToPlayer.UsedDate is not null;
            var isExpired = !isUsed && voucherDetail.VoucherToPlayer.ExpiredDate < now;
            var isAvailable = !isUsed && !isExpired;
            var isRedeemedByCounterPart = isUsed && voucherDetail.VoucherToPlayer.UsedBy != voucherDetail.VoucherToPlayer.PlayerId;
            var status = isUsed
                ? isRedeemedByCounterPart
                    ? "redeemed"
                    : "used"
                : isExpired
                    ? "expired"
                    : "available";

            var responseData = new PlayerVoucherDetailDto
            {
                VoucherToPlayerId = voucherDetail.VoucherToPlayer.Id,
                EventId = voucherDetail.VoucherToPlayer.EventId,
                PlayerId = voucherDetail.VoucherToPlayer.PlayerId,
                AcquiredDate = voucherDetail.VoucherToPlayer.CreatedDate,
                ExpiredDate = voucherDetail.VoucherToPlayer.ExpiredDate,
                UsedDate = voucherDetail.VoucherToPlayer.UsedDate,
                UsedBy = voucherDetail.VoucherToPlayer.UsedBy,
                IsAvailable = isAvailable,
                IsExpired = isExpired,
                IsUsed = isUsed,
                IsRedeemedByCounterPart = isRedeemedByCounterPart,
                Status = status,
                Voucher = new VoucherDto
                {
                    Id = voucherDetail.Voucher.Id,
                    Title = voucherDetail.Voucher.Title,
                    ImageUrl = voucherDetail.Voucher.ImageUrl,
                    Value = voucherDetail.Voucher.Value,
                    TotalQuantity = voucherDetail.Voucher.TotalQuantity,
                    ExpiredDate = voucherDetail.Voucher.ExpiredDate,
                    RedemptionInstructions = voucherDetail.Voucher.RedemptionInstructions,
                    IssuedQuantity = voucherDetail.IssuedQuantity,
                    UsedQuantity = voucherDetail.UsedQuantity,
                    RemainingQuantity = voucherDetail.Voucher.TotalQuantity.HasValue
                        ? Math.Max(voucherDetail.Voucher.TotalQuantity.Value - voucherDetail.IssuedQuantity, 0)
                        : int.MaxValue
                }
            };

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
