using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.PlayerCommands.UseVoucher;

public class UseVoucherHandler : IRequestHandler<UseVoucherCommand, BaseResponse<UseVoucherDto>>
{
    private readonly ILogger<UseVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public UseVoucherHandler(ILogger<UseVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<UseVoucherDto>> Handle(UseVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(UseVoucherHandler)}.{nameof(Handle)} UserId = {userId}, VoucherToPlayerId = {request.VoucherToPlayerId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<UseVoucherDto>();
        
        try 
        {
            var now = DateTime.UtcNow;
            var voucherToPlayer = await _unitOfWork.VoucherToPlayers
                .Where(v => 
                    v.Id == request.VoucherToPlayerId 
                    && v.PlayerId == userId 
                    && !v.IsDeleted)
                .Select(v => new
                {
                    v.Id,
                    v.VoucherId,
                    v.PlayerId,
                    v.ExpiredDate,
                    v.UsedDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (voucherToPlayer == null || voucherToPlayer.UsedDate is not null)
            {
                response.ToBadRequestResponse("Voucher not found or already used");
                return response;
            }

            if (voucherToPlayer.ExpiredDate < now)
            {
                response.ToBadRequestResponse("Voucher has expired");
                return response;
            }

            var affectedRows = await _unitOfWork.VoucherToPlayers
                .Where(v => v.Id == request.VoucherToPlayerId
                            && v.PlayerId == userId
                            && !v.IsDeleted
                            && v.UsedDate == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(v => v.UsedDate, now)
                    .SetProperty(v => v.UsedBy, userId)
                    .SetProperty(v => v.ModifiedDate, now), cancellationToken);

            if (affectedRows == 0)
            {
                response.ToBadRequestResponse("Voucher not found or already used");
                return response;
            }
            
            var responseDto = new UseVoucherDto
            {
                Id = voucherToPlayer.Id,
                VoucherId = voucherToPlayer.VoucherId,
                PlayerId = voucherToPlayer.PlayerId,
                UsedBy = userId,
                UsedDate = now
            };
            response.ToSuccessResponse(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }
        
        return response;
    }
}
