using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.DeleteQuizSet;

public class DeleteQuizSetHandler: IRequestHandler<DeleteQuizSetCommand, BaseResponse<object>>
{
    private readonly ILogger<DeleteQuizSetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public DeleteQuizSetHandler(ILogger<DeleteQuizSetHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }


    public async Task<BaseResponse<object>> Handle(DeleteQuizSetCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName =
            $"{nameof(DeleteQuizSetHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<object>();

        try
        {
            var existedQuizSet = await _unitOfWork.QuizSets
                .Where(x =>
                    !x.IsDeleted
                    && x.CounterPartId == userId
                    && x.Id == request.QuizSetId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existedQuizSet is null)
            {
                _logger.LogWarning($"{methodName} QuizSet is not existed");
                response.ToBadRequestResponse("QuizSet is not existed");
                return response;
            }

            var isQuizSetInQuizSession = await _unitOfWork.QuizSessions
                .Where(x =>
                    !x.IsDeleted
                    && x.QuizSetId == request.QuizSetId)
                .AnyAsync(cancellationToken);

            if (isQuizSetInQuizSession)
            {
                _logger.LogWarning($"{methodName} QuizSet is in QuizSession");
                response.ToForbiddenResponse("QuizSet is in in-use");
                return response;
            }

            // Mark QuizSet as deleted
            existedQuizSet.IsDeleted = true;
            existedQuizSet.ModifiedDate = DateTime.UtcNow;

            _unitOfWork.QuizSets.Update(existedQuizSet);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            response.ToSuccessResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName} {ex.Message}");
            response.ToInternalErrorResponse();
        }
        return response;
    }
}
