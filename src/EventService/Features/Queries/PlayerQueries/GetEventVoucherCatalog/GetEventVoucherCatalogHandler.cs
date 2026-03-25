using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetEventVoucherCatalog;

public class GetEventVoucherCatalogHandler : IRequestHandler<GetEventVoucherCatalogQuery, BaseResponse<GetEventVoucherCatalogResponse>>
{
    private readonly ILogger<GetEventVoucherCatalogHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;

    public GetEventVoucherCatalogHandler(
        ILogger<GetEventVoucherCatalogHandler> logger,
        IUnitOfWork unitOfWork,
        ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetEventVoucherCatalogResponse>> Handle(GetEventVoucherCatalogQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetEventVoucherCatalogHandler)}.{nameof(Handle)} UserId = {userId}, EventId = {request.EventId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetEventVoucherCatalogResponse>();

        try
        {
            var eventInfo = await _unitOfWork.Events
                .Where(x => !x.IsDeleted
                            && x.Id == request.EventId
                            && (x.Status == EventStatus.Approved || x.Status == EventStatus.InProgress))
                .Select(x => new
                {
                    x.Id,
                    x.ShakeVoucherId,
                    x.ShakePrice
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (eventInfo is null)
            {
                response.ToNotFoundResponse("Event not found");
                return response;
            }

            var now = DateTime.UtcNow;
            var shakeVoucherIds = new List<string?> { eventInfo.ShakeVoucherId };
            var quizVoucherIds = await _unitOfWork.QuizSessions
                .Where(x => !x.IsDeleted && x.EventId == request.EventId)
                .Select(x => x.VoucherId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var voucherIds = shakeVoucherIds
                .Concat(quizVoucherIds)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct()
                .ToList();

            var vouchers = await _unitOfWork.Vouchers
                .Where(x => !x.IsDeleted && voucherIds.Contains(x.Id))
                .Select(voucher => new
                {
                    Voucher = voucher,
                    IssuedQuantity = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id),
                    UsedQuantity = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id && vp.UsedDate != null),
                    PlayerOwnedCount = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id && vp.PlayerId == userId),
                    PlayerAvailableCount = _unitOfWork.VoucherToPlayers.GetAll().Count(vp => !vp.IsDeleted && vp.VoucherId == voucher.Id && vp.PlayerId == userId && vp.UsedDate == null && vp.ExpiredDate >= now)
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var responseData = new GetEventVoucherCatalogResponse
            {
                EventId = eventInfo.Id,
                ShakeVoucherId = eventInfo.ShakeVoucherId,
                ShakePrice = eventInfo.ShakePrice,
                Vouchers = vouchers
                    .OrderByDescending(x => x.Voucher.Id == eventInfo.ShakeVoucherId)
                    .ThenByDescending(x => x.Voucher.Value)
                    .ThenBy(x => x.Voucher.Title)
                    .Select(x => new EventVoucherCatalogItemDto
                    {
                        IsShakeVoucher = x.Voucher.Id == eventInfo.ShakeVoucherId,
                        PlayerOwnedCount = x.PlayerOwnedCount,
                        PlayerAvailableCount = x.PlayerAvailableCount,
                        Voucher = new VoucherDto
                        {
                            Id = x.Voucher.Id,
                            Title = x.Voucher.Title,
                            ImageUrl = x.Voucher.ImageUrl,
                            Value = x.Voucher.Value,
                            TotalQuantity = x.Voucher.TotalQuantity,
                            ExpiredDate = x.Voucher.ExpiredDate,
                            RedemptionInstructions = x.Voucher.RedemptionInstructions,
                            IssuedQuantity = x.IssuedQuantity,
                            UsedQuantity = x.UsedQuantity,
                            RemainingQuantity = x.Voucher.TotalQuantity.HasValue
                                ? Math.Max(x.Voucher.TotalQuantity.Value - x.IssuedQuantity, 0)
                                : int.MaxValue
                        }
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
