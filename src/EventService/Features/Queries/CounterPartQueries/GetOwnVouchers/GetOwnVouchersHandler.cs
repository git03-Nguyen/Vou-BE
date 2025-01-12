using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnVouchers;

public class GetOwnVouchersHandler : IRequestHandler<GetOwnVouchersQuery, BaseResponse<GetOwnVouchersResponse>>
{
    private readonly ILogger<GetOwnVouchersHandler> _logger;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    public GetOwnVouchersHandler(ILogger<GetOwnVouchersHandler> logger, ICustomHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GetOwnVouchersResponse>> Handle(GetOwnVouchersQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnVouchersHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetOwnVouchersResponse>();

        try
        {
            var vouchers = await
                (
                    from voucher in _unitOfWork.Vouchers.GetAll()
                    where !voucher.IsDeleted && voucher.CounterPartId == userId
                    select new VoucherDto
                    {
                        Id = voucher.Id,
                        Title = voucher.Title,
                        ImageUrl = voucher.ImageUrl,
                        Value = voucher.Value
                    }
                )
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var responseData = new GetOwnVouchersResponse { Vouchers = vouchers };
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