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
            var isVoucherExisted = await _unitOfWork.Vouchers
                .Where(x => 
                    !x.IsDeleted
                    && x.CounterPartId == userId
                    && x.Title == request.Title)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (isVoucherExisted)
            {
                _logger.LogWarning($"{methodName} Voucher is existed");
                response.ToBadRequestResponse("Voucher is existed");
                return response;
            }
            
            var newVoucher = new Voucher
            {
                Title = request.Title,
                CounterPartId = userId,
                ImageUrl = request.ImageUrl,
                Value = request.Value
            };
            
            await _unitOfWork.Vouchers.AddAsync(newVoucher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var responseData = new VoucherDto
            {
                Id = newVoucher.Id,
                Title = newVoucher.Title,
                ImageUrl = newVoucher.ImageUrl,
                Value = newVoucher.Value,
            };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}