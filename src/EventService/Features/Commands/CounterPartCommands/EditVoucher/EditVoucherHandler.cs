using System.Text.Json;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.EditVoucher;

public class EditVoucherHandler : IRequestHandler<EditVoucherCommand, BaseResponse<VoucherDto>>
{
    private readonly ILogger<EditVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public EditVoucherHandler(ILogger<EditVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<VoucherDto>> Handle(EditVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(EditVoucherHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<VoucherDto>();

        try
        {
            var voucher = await _unitOfWork.Vouchers
                .Where(v => !v.IsDeleted && v.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (voucher == null)
            {
                response.ToNotFoundResponse("Voucher not found");
                return response;
            }

            if (voucher.CounterPartId != userId)
            {
                response.ToForbiddenResponse("You are not allowed to edit this voucher");
                return response;
            }

            voucher.ImageUrl = request.ImageUrl?.Trim() ?? voucher.ImageUrl;
            voucher.Title = request.Title?.Trim() ?? voucher.Title;
            voucher.Value = request.Value ?? voucher.Value;
            voucher.TotalQuantity = request.TotalQuantity ?? voucher.TotalQuantity;
            voucher.ExpiredDate = request.ExpiredDate?.ToUniversalTime() ?? voucher.ExpiredDate;
            voucher.RedemptionInstructions = request.RedemptionInstructions?.Trim() ?? voucher.RedemptionInstructions;
            voucher.ModifiedDate = DateTime.UtcNow;

            var issuedQuantity = await _unitOfWork.VoucherToPlayers
                .Where(x => !x.IsDeleted && x.VoucherId == voucher.Id)
                .CountAsync(cancellationToken);

            if (voucher.TotalQuantity.HasValue && voucher.TotalQuantity.Value < issuedQuantity)
            {
                response.ToBadRequestResponse("TotalQuantity cannot be less than already issued vouchers");
                return response;
            }

            _unitOfWork.Vouchers.Update(voucher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var usedQuantity = await _unitOfWork.VoucherToPlayers
                .Where(x => !x.IsDeleted && x.VoucherId == voucher.Id && x.UsedDate != null)
                .CountAsync(cancellationToken);

            var remainingQuantity = voucher.TotalQuantity.HasValue
                ? Math.Max(voucher.TotalQuantity.Value - issuedQuantity, 0)
                : int.MaxValue;

            var responseData = new VoucherDto
            {
                Id = voucher.Id,
                ImageUrl = voucher.ImageUrl,
                Title = voucher.Title,
                Value = voucher.Value,
                TotalQuantity = voucher.TotalQuantity,
                ExpiredDate = voucher.ExpiredDate,
                RedemptionInstructions = voucher.RedemptionInstructions,
                IssuedQuantity = issuedQuantity,
                UsedQuantity = usedQuantity,
                RemainingQuantity = remainingQuantity,
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
