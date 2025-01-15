using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.PlayerCommands.BuyVoucher;

public class BuyVoucherHandler : IRequestHandler<BuyVoucherCommand, BaseResponse<BuyVoucherDto>>
{
    private readonly ILogger<BuyVoucherHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public BuyVoucherHandler(ILogger<BuyVoucherHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<BuyVoucherDto>> Handle(BuyVoucherCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(BuyVoucherHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<BuyVoucherDto>();

        try
        {
            // Fetch event entity
            var eventEntity = await _unitOfWork.Events
                .Where(e => e.Id == request.EventId && e.Status == EventStatus.InProgress && e.ShakeVoucherId != null)
                .FirstOrDefaultAsync(cancellationToken);

            if (eventEntity == null)
            {
                response.ToBadRequestResponse("Event not found or not in progress or no voucher available");
                return response;
            }

            // Fetch voucher entity
            var existedVoucher = await _unitOfWork.Vouchers
                .Where(v => v.Id == eventEntity.ShakeVoucherId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existedVoucher == null)
            {
                response.ToBadRequestResponse("Voucher not found");
                return response;
            }

            // Use HttpClient to fetch ticket event data
            var ticketEventApiUrl = $"http://localhost:5000/GameService/api/1/Player/GetOwnTickets/{eventEntity.Id}"; // Replace with actual API URL
            var httpClient = new HttpClient();

            HttpResponseMessage httpResponse;
            try
            {
                // Add Authorization header with token
                var token = _contextAccessor.GetCurrentJwtToken(); 
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                httpResponse = await httpClient.GetAsync(ticketEventApiUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{methodName} Error calling ticket event API: {ex.Message}");
                response.ToInternalErrorResponse();
                return response;
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning($"{methodName} Failed to fetch ticket event data. Status Code: {httpResponse.StatusCode}");
                response.ToBadRequestResponse("Failed to fetch ticket event data");
                return response;
            }

            var ticketEventJson = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var ticketEvent = JsonSerializer.Deserialize<PlayerShakeDto>(ticketEventJson);

            var shakeData = ticketEvent?.Diamonds;

            if (shakeData == null)
            {
                response.ToBadRequestResponse("No shake data found");
                return response;
            }

            if (shakeData < eventEntity.ShakeAverageDiamond)
            {
                response.ToBadRequestResponse("Insufficient diamonds");
                return response;
            }

            shakeData -= eventEntity.ShakeAverageDiamond;
            var toPlayer = new VoucherToPlayer
            {
                EventId = eventEntity.Id,
                PlayerId = userId,
                Description = existedVoucher.Title,
                VoucherId = existedVoucher.Id
            };

            await _unitOfWork.VoucherToPlayers.AddAsync(toPlayer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            response.ToSuccessResponse(new BuyVoucherDto
            {
                Id = toPlayer.Id,
                Diamonds = shakeData.Value,
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
            _logger.LogError($"{methodName} {ex.Message}");
            response.ToInternalErrorResponse();
            return response;
        }
    }
}
