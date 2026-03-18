using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.CreateVoucher;

public class CreateVoucherHandler: IRequestHandler<CreateVoucherCommand, BaseResponse<VoucherDto>>
{
    private readonly ILogger<CreateVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public CreateVoucherHandler(ILogger<CreateVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<VoucherDto>> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        _logger.LogInformation($"UserId: {userId}");
        var methodName = $"{nameof(CreateVoucherHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<VoucherDto>();

        try
        {
            var normalizedTitle = request.Title.Trim();
            var normalizedTitleKey = normalizedTitle.ToLower();
            var normalizedImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? null
                : request.ImageUrl.Trim();
            var normalizedRedemptionInstructions = string.IsNullOrWhiteSpace(request.RedemptionInstructions)
                ? null
                : request.RedemptionInstructions.Trim();

            var isVoucherExisted = await _unitOfWork.Vouchers
                .Where(x => 
                    !x.IsDeleted
                    && x.CounterPartId == userId
                    && x.Title.Trim().ToLower() == normalizedTitleKey)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (isVoucherExisted)
            {
                _logger.LogWarning($"{methodName} Voucher title already exists");
                response.ToBadRequestResponse("Voucher title already exists");
                return response;
            }
            
            var newVoucher = new Voucher
            {
                Title = normalizedTitle,
                CounterPartId = userId,
                ImageUrl = normalizedImageUrl,
                Value = request.Value,
                TotalQuantity = request.TotalQuantity,
                ExpiredDate = request.ExpiredDate?.ToUniversalTime(),
                RedemptionInstructions = normalizedRedemptionInstructions
            };
            
            await _unitOfWork.Vouchers.AddAsync(newVoucher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var responseData = new VoucherDto
            {
                Id = newVoucher.Id,
                Title = newVoucher.Title,
                ImageUrl = newVoucher.ImageUrl,
                Value = newVoucher.Value,
                TotalQuantity = newVoucher.TotalQuantity,
                ExpiredDate = newVoucher.ExpiredDate,
                RedemptionInstructions = newVoucher.RedemptionInstructions,
                IssuedQuantity = 0,
                UsedQuantity = 0,
                RemainingQuantity = newVoucher.TotalQuantity ?? int.MaxValue,
            };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}
