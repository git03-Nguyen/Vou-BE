using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvent;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.AddQuizSession;

public class AddQuizSessionHandler: IRequestHandler<AddQuizSessionCommand, BaseResponse<FullEventDto>>
{
    private readonly ILogger<AddQuizSessionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IMediator _mediator;
    public AddQuizSessionHandler(ILogger<AddQuizSessionHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor, IMediator mediator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
        _mediator = mediator;
    }

    public async Task<BaseResponse<FullEventDto>> Handle(AddQuizSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(AddQuizSessionHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<FullEventDto>();

        try
        {
            var isEventExisted = await _unitOfWork.Events
                .Where(x => !x.IsDeleted 
                            && x.Id == request.EventId
                            && x.CounterPartId == userId)
                .AsNoTracking()
                .AnyAsync(cancellationToken);
            if (!isEventExisted)
            {
                _logger.LogWarning($"{methodName} Event does not exist");
                response.ToNotFoundResponse();
                return response;
            }
            
            var isQuizSetExisted = await _unitOfWork.QuizSets
                .Where(x => !x.IsDeleted && x.Id == request.QuizSetId && x.CounterPartId == userId)
                .AsNoTracking()
                .AnyAsync(cancellationToken);
            if (!isQuizSetExisted)
            {
                _logger.LogWarning($"{methodName} Quiz set does not exist");
                response.ToNotFoundResponse();
                return response;
            }

            var isVoucherExisted = await _unitOfWork.Vouchers
                .Where(x => !x.IsDeleted && x.Id == request.VoucherId && x.CounterPartId == userId)
                .AsNoTracking()
                .AnyAsync(cancellationToken);
            if (!isVoucherExisted)
            {
                _logger.LogWarning($"{methodName} Voucher does not exist");
                response.ToNotFoundResponse();
                return response;
            }
            
            var quizSessions = await _unitOfWork.QuizSessions
                .Where(x => !x.IsDeleted 
                            && x.EventId == request.EventId 
                            && x.Status == QuizSessionStatus.Pending)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (quizSessions.Any(x => Math.Abs((x.StartTime - request.StartTime).TotalHours) < 1))
            {
                _logger.LogWarning($"{methodName} Quiz session is overlapped");
                response.ToBadRequestResponse("Quiz session is overlapped");
                return response;
            }
            
            var newQuizSession = new QuizSession
            {
                EventId = request.EventId,
                VoucherId = request.VoucherId,
                StartTime = request.StartTime,
                TakeTop = request.TakeTop,
                QuizSetId = request.QuizSetId
            };
            
            await _unitOfWork.QuizSessions.AddAsync(newQuizSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var query = new GetOwnEventQuery { EventId = request.EventId };
            var fullEvent = await _mediator.Send(query, cancellationToken);
            response.ToSuccessResponse(fullEvent.Data);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}