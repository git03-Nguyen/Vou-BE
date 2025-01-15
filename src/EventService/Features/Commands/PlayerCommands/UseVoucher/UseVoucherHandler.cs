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
            var voucherToPlayer = await _unitOfWork.VoucherToPlayers
                .Where(v => 
                    v.Id == request.VoucherToPlayerId 
                    && v.PlayerId == userId 
                    && !v.IsDeleted 
                    && v.UsedDate == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (voucherToPlayer == null)
            {
                response.ToBadRequestResponse("Voucher not found or already used");
                return response;
            }
            
            voucherToPlayer.UsedDate = DateTime.UtcNow;
            voucherToPlayer.UsedBy = userId;
            _unitOfWork.VoucherToPlayers.Update(voucherToPlayer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var responseDto = new UseVoucherDto
            {
                Id = voucherToPlayer.Id,
                UsedDate = voucherToPlayer.UsedDate.Value
            };
            response.ToSuccessResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName} Has error: {ex.Message}");
            response.ToInternalErrorResponse();
        }
        
        return response;
    }
}