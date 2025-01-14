using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetAllCounterParts;

public class GetAllCounterPartsHandler : IRequestHandler<GetAllCounterPartsQuery, BaseResponse<GetAllCounterPartsResponse>>
{
    private readonly ILogger<GetAllCounterPartsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetAllCounterPartsHandler(ILogger<GetAllCounterPartsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetAllCounterPartsResponse>> Handle(GetAllCounterPartsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetAllCounterPartsHandler)}.{nameof(Handle)} UserId = {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetAllCounterPartsResponse>();
        
        try
        {
            var counterParts = await _unitOfWork.CounterParts
                .Where(x => !x.IsBlocked)
                .Select(x => new CounterPartDto
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Address = x.Address,
                    FullName = x.FullName,
                    Field = x.Field
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var responseData = new GetAllCounterPartsResponse { CounterParts = counterParts };
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