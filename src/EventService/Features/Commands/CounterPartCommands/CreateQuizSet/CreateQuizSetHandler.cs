using System.Data;
using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvent;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.CreateQuizSet;

public class CreateQuizSetHandler: IRequestHandler<CreateQuizSetCommand, BaseResponse<QuizSetDto>>
{
    private readonly ILogger<CreateQuizSetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public CreateQuizSetHandler(ILogger<CreateQuizSetHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<QuizSetDto>> Handle(CreateQuizSetCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(CreateQuizSetHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<QuizSetDto>();

        try
        {
            await using var transaction = await _unitOfWork.OpenTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var isQuizSetExisted = await _unitOfWork.QuizSets
                .Where(x => 
                    !x.IsDeleted
                    && x.CounterPartId == userId
                    && x.Title == request.Title)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (isQuizSetExisted)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning($"{methodName} QuizSet is existed");
                response.ToBadRequestResponse("QuizSet is existed");
                return response;
            }
            
            var newQuizSet = new QuizSet
            {
                Title = request.Title,
                CounterPartId = userId,
                ImageUrl = request.ImageUrl,
                QuizesSerialized = JsonSerializer.Serialize(request.Quizes)
            };
            
            await _unitOfWork.QuizSets.AddAsync(newQuizSet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            var responseData = new QuizSetDto
            {
                Id = newQuizSet.Id,
                Title = newQuizSet.Title,
                ImageUrl = newQuizSet.ImageUrl,
                QuizesSerialized = JsonSerializer.Serialize(request.Quizes)
            };
            response.ToSuccessResponse(responseData);
        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.SerializationFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Quiz set creation serialization conflict");
            response.ToBadRequestResponse("QuizSet is existed");
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(e, $"{methodName} Quiz set creation serialization conflict during save");
            response.ToBadRequestResponse("QuizSet is existed");
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}