using System.Data;
using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Contracts;
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

    public BuyVoucherHandler(
        ILogger<BuyVoucherHandler> logger,
        IUnitOfWork unitOfWork,
        ICustomHttpContextAccessor contextAccessor,
        IServiceInvocationService serviceInvocationService)
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
            await using var transaction = await _unitOfWork.OpenTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var @event = await
                (
                    from e in _unitOfWork.Events.GetAll()
                    join v in _unitOfWork.Vouchers.GetAll()
                        on e.ShakeVoucherId equals v.Id
                    where e.Id == request.EventId
                          && !e.IsDeleted
                          && !v.IsDeleted
                          && e.ShakeVoucherId != null
                    select new
                    {
                        Event = e,
                        Voucher = v
                    }

                )
                .FirstOrDefaultAsync(cancellationToken);

            if (@event is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Event not found or has no shake voucher configured");
                return response;
            }

            var now = DateTime.UtcNow;
            if (@event.Event.StartDate > now || @event.Event.EndDate < now)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Event is not active");
                return response;
            }

            if (@event.Voucher.ExpiredDate.HasValue && @event.Voucher.ExpiredDate.Value <= now)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Voucher has expired");
                return response;
            }

            var issuedQuantity = await _unitOfWork.VoucherToPlayers
                .Where(x => !x.IsDeleted && x.VoucherId == @event.Voucher.Id)
                .CountAsync(cancellationToken);

            if (@event.Voucher.TotalQuantity.HasValue && issuedQuantity >= @event.Voucher.TotalQuantity.Value)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Voucher is out of stock");
                return response;
            }

            const string appId = "gameservice";
            var diamondRequest = new PlayerTicketDiamondRequest
            {
                PlayerId = userId,
                EventId = @event.Event.Id
            };
            const string diamondRequestMethod = "Internal/Player/GetPlayerTicketDiamond";
            var diamondResponse = await
                _serviceInvocationService.InvokeServiceAsync<PlayerTicketDiamondRequest, PlayerTicketDiamondResponse>(
                    HttpMethod.Post,
                    appId,
                    diamondRequestMethod,
                    diamondRequest,
                    cancellationToken);

            if (diamondResponse is null || !diamondResponse.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToInternalErrorResponse("Failed to get ticket data");
                return response;
            }

            var totalDiamonds = diamondResponse.Diamonds;
            if (!@event.Event.ShakePrice.HasValue)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Event shake price is not configured");
                return response;
            }

            if (totalDiamonds < @event.Event.ShakePrice)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToBadRequestResponse("Insufficient diamonds");
                return response;
            }

            totalDiamonds -= @event.Event.ShakePrice.Value;
            var toPlayer = new VoucherToPlayer
            {
                EventId = @event.Event.Id,
                PlayerId = userId,
                Description = @event.Voucher.Title,
                VoucherId = @event.Voucher.Id,
                ExpiredDate = @event.Voucher.ExpiredDate ?? @event.Event.EndDate,
                CreatedBy = userId,
                CreatedDate = now,
                ModifiedDate = now
            };

            await _unitOfWork.VoucherToPlayers.AddAsync(toPlayer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updateDiamondRequest = new UpdatePlayerDiamondRequest
            {
                PlayerId = userId,
                EventId = request.EventId,
                Diamonds = totalDiamonds
            };
            const string updateDiamondRequestMethod = "Internal/Player/UpdatePlayerDiamond";
            var updateDiamondResponse = await
                _serviceInvocationService.InvokeServiceAsync<UpdatePlayerDiamondRequest, BaseResponse>(
                    HttpMethod.Post,
                    appId,
                    updateDiamondRequestMethod,
                    updateDiamondRequest,
                    cancellationToken);

            if (updateDiamondResponse is null || updateDiamondResponse.Status != 200)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                response.ToInternalErrorResponse("Failed to update player diamonds");
                return response;
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var updatedIssuedQuantity = issuedQuantity + 1;
            response.ToSuccessResponse(new BuyVoucherDto
            {
                VoucherToPlayerId = toPlayer.Id,
                Diamonds = totalDiamonds,
                Voucher = new VoucherDto
                {
                    Id = @event.Voucher.Id,
                    Title = @event.Voucher.Title,
                    ImageUrl = @event.Voucher.ImageUrl,
                    Value = @event.Voucher.Value,
                    TotalQuantity = @event.Voucher.TotalQuantity,
                    ExpiredDate = @event.Voucher.ExpiredDate,
                    RedemptionInstructions = @event.Voucher.RedemptionInstructions,
                    IssuedQuantity = updatedIssuedQuantity,
                    UsedQuantity = await _unitOfWork.VoucherToPlayers
                        .Where(x => !x.IsDeleted && x.VoucherId == @event.Voucher.Id && x.UsedDate != null)
                        .CountAsync(cancellationToken),
                    RemainingQuantity = @event.Voucher.TotalQuantity.HasValue
                        ? Math.Max(@event.Voucher.TotalQuantity.Value - updatedIssuedQuantity, 0)
                        : int.MaxValue
                }
            });

            return response;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.SerializationFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(ex, $"{methodName} Buy voucher serialization conflict");
            response.ToBadRequestResponse("Voucher stock changed, please retry");
            return response;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(ex, $"{methodName} Buy voucher serialization conflict during save");
            response.ToBadRequestResponse("Voucher stock changed, please retry");
            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, $"{methodName} Has error {ex.Message}");
            response.ToInternalErrorResponse();
            return response;
        }
    }
}
