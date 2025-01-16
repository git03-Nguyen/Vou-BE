using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.ServiceInvocations;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;
using Shared.Services.ServiceInvocation;

namespace EventService.Features.Commands.PlayerCommands.BuyVoucher;

public class BuyVoucherHandler : IRequestHandler<BuyVoucherCommand, BaseResponse<BuyVoucherDto>>
{
    private readonly ILogger<BuyVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IServiceInvocationService _serviceInvocationService;
    public BuyVoucherHandler(ILogger<BuyVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor, IServiceInvocationService serviceInvocationService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
        _serviceInvocationService = serviceInvocationService;
    }

    public async Task<BaseResponse<BuyVoucherDto>> Handle(BuyVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(BuyVoucherHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<BuyVoucherDto>();

        try
        {
            var @event = await
                (
                    from e in _unitOfWork.Events.GetAll()
                    join v in _unitOfWork.Vouchers.GetAll()
                        on e.ShakeVoucherId equals v.Id
                    where
                        e.Id == request.EventId
                        && e.Status == EventStatus.InProgress
                        && e.ShakeVoucherId != null
                    select new
                    {
                        e.Id,
                        e.ShakePrice,
                        e.ShakeVoucherId
                    }

                )
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
                

            if (@event is null)
            {
                response.ToBadRequestResponse("Event not found or not in progress or has no shake game");
                return response;
            }

            // Fetch voucher entity
            var existedVoucher = await _unitOfWork.Vouchers
                .Where(v => v.Id == @event.ShakeVoucherId)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.ImageUrl,
                    x.Value
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (existedVoucher is null)
            {
                response.ToBadRequestResponse("Voucher not found");
                return response;
            }
            
            // Go to GameService to get own ticket
            const string appId = "gameservice";
            var diamondRequest = new PlayerTicketDiamondRequest
            {
                PlayerId = userId,
                EventId = @event.Id
            };
            const string diamondRequestMethod = "Internal/Player/GetPlayerTicketDiamond";
            var diamondResponse = await 
                _serviceInvocationService.InvokeServiceAsync<PlayerTicketDiamondRequest, PlayerTicketDiamondResponse>
                (HttpMethod.Post, appId, diamondRequestMethod, diamondRequest, cancellationToken);

            if (diamondResponse is null || !diamondResponse.IsSuccess)
            {
                response.ToInternalErrorResponse("Failed to get ticket data");
                return response;
            }

            var totalDiamonds = diamondResponse.Diamonds;
            if (totalDiamonds < @event.ShakePrice)
            {
                response.ToBadRequestResponse("Insufficient diamonds");
                return response;
            }

            totalDiamonds -= @event.ShakePrice.Value;
            var toPlayer = new VoucherToPlayer
            {
                EventId = @event.Id,
                PlayerId = userId,
                Description = existedVoucher.Title,
                VoucherId = existedVoucher.Id,
                ExpiredDate = DateTime.Now.AddDays(30)
            };
            
            // Deduct diamonds
            await _unitOfWork.VoucherToPlayers.AddAsync(toPlayer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Update player diamonds on GameService
            var updateDiamondRequest = new UpdatePlayerDiamondRequest
            {
                PlayerId = userId,
                EventId = request.EventId,
                Diamonds = totalDiamonds
            };
            const string updateDiamondRequestMethod = "Internal/Player/UpdatePlayerDiamond";
            var updateDiamondResponse = await 
                _serviceInvocationService.InvokeServiceAsync<UpdatePlayerDiamondRequest, BaseResponse>
                (HttpMethod.Post, appId, updateDiamondRequestMethod, updateDiamondRequest, cancellationToken);

            response.ToSuccessResponse(new BuyVoucherDto
            {
                VoucherToPlayerId = toPlayer.Id,
                Diamonds = totalDiamonds,
                Voucher = new VoucherDto
                {
                    Id = existedVoucher.Id,
                    Title = existedVoucher.Title,
                    ImageUrl = existedVoucher.ImageUrl,
                    Value = existedVoucher.Value
                }
               
            });
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName} Has error {ex.Message}");
            response.ToInternalErrorResponse();
            return response;
        }
    }
}
