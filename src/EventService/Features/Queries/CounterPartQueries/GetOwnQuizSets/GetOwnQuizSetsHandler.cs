using System.Text.Json;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnQuizSets;

public class GetOwnQuizSetsHandler : IRequestHandler<GetOwnQuizSetsQuery, BaseResponse<GetOwnQuizSetsQueryResponse>>
{
    private readonly ILogger<GetOwnQuizSetsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetOwnQuizSetsHandler(ILogger<GetOwnQuizSetsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetOwnQuizSetsQueryResponse>> Handle(GetOwnQuizSetsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetOwnQuizSetsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetOwnQuizSetsQueryResponse>();

        try
        {
            var quizSets = await
            (
                from quizSet in _unitOfWork.QuizSets.GetAll()
                where !quizSet.IsDeleted && quizSet.CounterPartId == userId
                select new QuizSetDto
                {
                    Id = quizSet.Id,
                    Title = quizSet.Title,
                    ImageUrl = quizSet.ImageUrl,
                    QuizesSerialized = quizSet.QuizesSerialized
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new GetOwnQuizSetsQueryResponse { QuizSets = quizSets };
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