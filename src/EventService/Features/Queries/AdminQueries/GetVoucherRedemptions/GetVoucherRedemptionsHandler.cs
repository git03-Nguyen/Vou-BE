using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace EventService.Features.Queries.AdminQueries.GetVoucherRedemptions;

public class GetVoucherRedemptionsHandler : IRequestHandler<GetVoucherRedemptionsQuery, BaseResponse<GetVoucherRedemptionsResponse>>
{
    private readonly ILogger<GetVoucherRedemptionsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetVoucherRedemptionsHandler(ILogger<GetVoucherRedemptionsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GetVoucherRedemptionsResponse>> Handle(GetVoucherRedemptionsQuery request, CancellationToken cancellationToken)
    {
        const string methodName = $"{nameof(GetVoucherRedemptionsHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetVoucherRedemptionsResponse>();

        try
        {
            var now = DateTime.UtcNow;
            var statusFilter = request.Status?.Trim().ToLowerInvariant();
            var search = request.Search?.Trim().ToLowerInvariant();
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 50;

            var voucherRedemptions = await
            (
                from voucherToPlayer in _unitOfWork.VoucherToPlayers.GetAll()
                join voucher in _unitOfWork.Vouchers.GetAll()
                    on voucherToPlayer.VoucherId equals voucher.Id
                join event_ in _unitOfWork.Events.GetAll()
                    on voucherToPlayer.EventId equals event_.Id
                join counterPart in _unitOfWork.CounterParts.GetAll()
                    on voucher.CounterPartId equals counterPart.Id
                join player in _unitOfWork.Players.GetAll()
                    on voucherToPlayer.PlayerId equals player.Id
                where !voucherToPlayer.IsDeleted
                      && !voucher.IsDeleted
                      && !event_.IsDeleted
                      && (string.IsNullOrWhiteSpace(request.EventId) || voucherToPlayer.EventId == request.EventId)
                      && (string.IsNullOrWhiteSpace(request.VoucherId) || voucherToPlayer.VoucherId == request.VoucherId)
                      && (string.IsNullOrWhiteSpace(request.CounterPartId) || voucher.CounterPartId == request.CounterPartId)
                      && (string.IsNullOrWhiteSpace(request.PlayerId) || voucherToPlayer.PlayerId == request.PlayerId)
                orderby voucherToPlayer.CreatedDate descending
                select new
                {
                    voucherToPlayer.Id,
                    voucherToPlayer.EventId,
                    EventName = event_.Name,
                    voucherToPlayer.VoucherId,
                    VoucherTitle = voucher.Title,
                    CounterPartId = counterPart.Id,
                    CounterPartName = counterPart.FullName,
                    voucherToPlayer.PlayerId,
                    PlayerName = player.FullName,
                    PlayerUserName = player.UserName,
                    AcquiredDate = voucherToPlayer.CreatedDate,
                    voucherToPlayer.ExpiredDate,
                    voucherToPlayer.UsedDate,
                    voucherToPlayer.UsedBy
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var filtered = voucherRedemptions
                .Select(x =>
                {
                    var isUsed = x.UsedDate is not null;
                    var isExpired = !isUsed && x.ExpiredDate < now;
                    var isAvailable = !isUsed && !isExpired;
                    var isRedeemedByCounterPart = isUsed && x.UsedBy != x.PlayerId;
                    var status = isUsed
                        ? isRedeemedByCounterPart
                            ? "redeemed"
                            : "used"
                        : isExpired
                            ? "expired"
                            : "available";

                    return new AdminVoucherRedemptionDto
                    {
                        VoucherToPlayerId = x.Id,
                        EventId = x.EventId,
                        EventName = x.EventName,
                        VoucherId = x.VoucherId,
                        VoucherTitle = x.VoucherTitle,
                        CounterPartId = x.CounterPartId,
                        CounterPartName = x.CounterPartName,
                        PlayerId = x.PlayerId,
                        PlayerName = x.PlayerName,
                        PlayerUserName = x.PlayerUserName,
                        AcquiredDate = x.AcquiredDate,
                        ExpiredDate = x.ExpiredDate,
                        UsedDate = x.UsedDate,
                        UsedBy = x.UsedBy,
                        IsAvailable = isAvailable,
                        IsExpired = isExpired,
                        IsUsed = isUsed,
                        IsRedeemedByCounterPart = isRedeemedByCounterPart,
                        Status = status
                    };
                })
                .Where(x => string.IsNullOrWhiteSpace(statusFilter) || x.Status == statusFilter)
                .Where(x => !request.AcquiredFrom.HasValue || (x.AcquiredDate.HasValue && x.AcquiredDate.Value >= request.AcquiredFrom.Value))
                .Where(x => !request.AcquiredTo.HasValue || (x.AcquiredDate.HasValue && x.AcquiredDate.Value <= request.AcquiredTo.Value))
                .Where(x => !request.UsedFrom.HasValue || (x.UsedDate.HasValue && x.UsedDate.Value >= request.UsedFrom.Value))
                .Where(x => !request.UsedTo.HasValue || (x.UsedDate.HasValue && x.UsedDate.Value <= request.UsedTo.Value))
                .Where(x => !request.ExpiredFrom.HasValue || x.ExpiredDate >= request.ExpiredFrom.Value)
                .Where(x => !request.ExpiredTo.HasValue || x.ExpiredDate <= request.ExpiredTo.Value)
                .Where(x => string.IsNullOrWhiteSpace(search)
                            || x.EventName.ToLower().Contains(search)
                            || x.VoucherTitle.ToLower().Contains(search)
                            || x.CounterPartName.ToLower().Contains(search)
                            || x.PlayerName.ToLower().Contains(search)
                            || (!string.IsNullOrWhiteSpace(x.PlayerUserName) && x.PlayerUserName.ToLower().Contains(search))
                            || x.VoucherToPlayerId.ToLower().Contains(search))
                .ToList();

            var totalCount = filtered.Count;
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
            var pagedItems = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var responseData = new GetVoucherRedemptionsResponse
            {
                TotalCount = totalCount,
                AvailableCount = filtered.Count(x => x.Status == "available"),
                UsedCount = filtered.Count(x => x.Status == "used"),
                RedeemedCount = filtered.Count(x => x.Status == "redeemed"),
                ExpiredCount = filtered.Count(x => x.Status == "expired"),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1 && totalCount > 0,
                HasNextPage = totalPages > 0 && pageNumber < totalPages,
                VoucherRedemptions = pagedItems
            };

            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}