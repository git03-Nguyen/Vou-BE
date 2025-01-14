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

            _unitOfWork.Vouchers.Update(voucher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var responseData = new VoucherDto
            {
                Id = voucher.Id,
                ImageUrl = voucher.ImageUrl,
                Title = voucher.Title,
                Value = voucher.Value,
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