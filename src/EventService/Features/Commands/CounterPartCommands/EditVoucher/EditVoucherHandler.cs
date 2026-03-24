using System.Data;
using System.Text.Json;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            var normalizedTitle = request.Title?.Trim();
            var normalizedTitleKey = normalizedTitle?.ToLower();
            var normalizedImageUrl = request.ImageUrl is null
                ? null
                : string.IsNullOrWhiteSpace(request.ImageUrl)
                    ? null
                    : request.ImageUrl.Trim();
            var normalizedRedemptionInstructions = request.RedemptionInstructions is null
                ? null
                : string.IsNullOrWhiteSpace(request.RedemptionInstructions)
                    ? null
                    : request.RedemptionInstructions.Trim();

            await using var transaction = await _unitOfWork.OpenTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var voucher = await _unitOfWork.Vouchers
                .Where(v => !v.IsDeleted && v.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (voucher == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToNotFoundResponse("Voucher not found");
                return response;
            }

            if (voucher.CounterPartId != userId)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToForbiddenResponse("You are not allowed to edit this voucher");
                return response;
            }

            if (normalizedTitleKey is not null)
            {
                var isDuplicateTitle = await _unitOfWork.Vouchers
                    .Where(v => !v.IsDeleted
                                && v.CounterPartId == userId
                                && v.Id != voucher.Id
                                && v.Title.Trim().ToLower() == normalizedTitleKey)
                    .AsNoTracking()
                    .AnyAsync(cancellationToken);

                if (isDuplicateTitle)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    response.ToBadRequestResponse("Voucher title already exists");
                    return response;
                }
            }

            if (request.ImageUrl is not null)
            {
                voucher.ImageUrl = normalizedImageUrl;
            }

            voucher.Title = normalizedTitle ?? voucher.Title;
            voucher.Value = request.Value ?? voucher.Value;
            voucher.TotalQuantity = request.TotalQuantity ?? voucher.TotalQuantity;
            voucher.ExpiredDate = request.ExpiredDate?.ToUniversalTime() ?? voucher.ExpiredDate;

            if (request.RedemptionInstructions is not null)
            {
                voucher.RedemptionInstructions = normalizedRedemptionInstructions;
            }

            voucher.ModifiedDate = DateTime.UtcNow;

            var issuedQuantity = await _unitOfWork.VoucherToPlayers
                .Where(x => !x.IsDeleted && x.VoucherId == voucher.Id)
                .CountAsync(cancellationToken);

            if (voucher.TotalQuantity.HasValue && voucher.TotalQuantity.Value < issuedQuantity)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("TotalQuantity cannot be less than already issued vouchers");
                return response;
            }

            _unitOfWork.Vouchers.Update(voucher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

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
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.SerializationFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Voucher edit serialization conflict");
            response.ToBadRequestResponse("Voucher title already exists");
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Voucher edit serialization conflict during save");
            response.ToBadRequestResponse("Voucher title already exists");
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
