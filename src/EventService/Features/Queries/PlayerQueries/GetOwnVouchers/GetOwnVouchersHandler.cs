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
            var now = DateTime.UtcNow;
            var vouchers = await
            (
                from voucherToPlayer in _unitOfWork.VoucherToPlayers.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on voucherToPlayer.VoucherId equals voucher.Id
                where !voucher.IsDeleted
                    && !voucherToPlayer.IsDeleted
                    && voucherToPlayer.PlayerId == userId
                orderby voucherToPlayer.CreatedDate descending
                let issuedQuantity = _unitOfWork.VoucherToPlayers.GetAll()
                    .Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id)
                let usedQuantity = _unitOfWork.VoucherToPlayers.GetAll()
                    .Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id && vp.UsedDate != null)
                select new
                {
                    voucherToPlayer.EventId,
                    VoucherId = voucher.Id,
                    voucher.Title,
                    voucher.Value,
                    voucher.ImageUrl,
                    voucher.TotalQuantity,
                    VoucherExpiredDate = voucher.ExpiredDate,
                    voucher.RedemptionInstructions,
                    IssuedQuantity = issuedQuantity,
                    UsedQuantity = usedQuantity,
                    voucherToPlayer.Id,
                    voucherToPlayer.CreatedDate,
                    voucherToPlayer.ExpiredDate,
                    voucherToPlayer.UsedDate,
                    voucherToPlayer.UsedBy,
                    voucherToPlayer.PlayerId
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new GetOwnVouchersResponse
            {
                Vouchers = vouchers
                    .GroupBy(x => new
                    {
                        x.EventId,
                        x.VoucherId,
                        x.Title,
                        x.Value,
                        x.ImageUrl,
                        x.TotalQuantity,
                        x.VoucherExpiredDate,
                        x.RedemptionInstructions,
                        x.IssuedQuantity,
                        x.UsedQuantity
                    })
                    .Select(g => new OwnVoucherDto
                    {
                        EventId = g.Key.EventId,
                        Count = g.Count(),
                        AvailableCount = g.Count(x => x.UsedDate is null && x.ExpiredDate >= now),
                        UsedCount = g.Count(x => x.UsedDate is not null),
                        ExpiredCount = g.Count(x => x.UsedDate is null && x.ExpiredDate < now),
                        Voucher = new VoucherDto
                        {
                            Id = g.Key.VoucherId,
                            Title = g.Key.Title,
                            Value = g.Key.Value,
                            ImageUrl = g.Key.ImageUrl,
                            TotalQuantity = g.Key.TotalQuantity,
                            ExpiredDate = g.Key.VoucherExpiredDate,
                            RedemptionInstructions = g.Key.RedemptionInstructions,
                            IssuedQuantity = g.Key.IssuedQuantity,
                            UsedQuantity = g.Key.UsedQuantity,
                            RemainingQuantity = g.Key.TotalQuantity.HasValue
                                ? Math.Max(g.Key.TotalQuantity.Value - g.Key.IssuedQuantity, 0)
                                : int.MaxValue
                        },
                        Items = g.Select(x =>
                        {
                            var isUsed = x.UsedDate is not null;
                            var isExpired = !isUsed && x.ExpiredDate < now;
                            var isAvailable = !isUsed && !isExpired;
                            var isRedeemedByCounterPart = isUsed && x.UsedBy != x.PlayerId;

                            return new OwnVoucherItemDto
                            {
                                VoucherToPlayerId = x.Id,
                                AcquiredDate = x.CreatedDate,
                                ExpiredDate = x.ExpiredDate,
                                UsedDate = x.UsedDate,
                                UsedBy = x.UsedBy,
                                IsAvailable = isAvailable,
                                IsExpired = isExpired,
                                IsRedeemedByCounterPart = isRedeemedByCounterPart,
                                Status = isUsed
                                    ? isRedeemedByCounterPart
                                        ? "redeemed"
                                        : "used"
                                    : isExpired
                                        ? "expired"
                                        : "available"
                            };
                        }).ToList()
                    })
                    .ToList()
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
