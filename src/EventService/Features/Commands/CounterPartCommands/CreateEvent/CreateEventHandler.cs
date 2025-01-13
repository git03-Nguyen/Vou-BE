using System.Text.Json;
using EventService.Data.Models;
using EventService.DTOs;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvent;
using EventService.Repositories;
using EventService.Services.PubSubService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.CounterPartCommands.CreateEvent;

public class CreateEventHandler: IRequestHandler<CreateEventCommand, BaseResponse<FullEventDto>>
{
    private readonly ILogger<CreateEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IMediator _mediator;
    private readonly IEventPublishService _eventPublishService;
    public CreateEventHandler(ILogger<CreateEventHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor, IMediator mediator, IEventPublishService eventPublishService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
        _mediator = mediator;
        _eventPublishService = eventPublishService;
    }

    public async Task<BaseResponse<FullEventDto>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(CreateEventHandler)}.{nameof(Handle)} UserId = {userId}, Payload = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<FullEventDto>();
        var transaction = default(IDbContextTransaction);

        try
        {
            var isEventExisted = await _unitOfWork.Events
                .Where(x => 
                    !x.IsDeleted
                    && x.CounterPartId == userId
                    && x.Name == request.Name)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (isEventExisted)
            {
                _logger.LogWarning($"{methodName} Event is existed");
                response.ToBadRequestResponse("Event is existed or start date is duplicated");
                return response;
            }
            
            var newEvent = new Event
            {
                Name = request.Name,
                CounterPartId = userId,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                StartDate = request.StartDate,
                CreatedBy = userId,
                ShakePrice = request.ShakeSession?.Price ?? null,
                ShakeVoucherId = request.ShakeSession?.VoucherId ?? null,
                ShakeWinRate = request.ShakeSession?.WinRate ?? null,
                ShakeAverageDiamond = request.ShakeSession?.AverageDiamond ?? null
            };
            
            var quizSessions = request.QuizSessions?
                .Select(x => new QuizSession
                {
                    EventId = newEvent.Id,
                    VoucherId = x.VoucherId,
                    StartTime = x.StartTime,
                    TakeTop = x.TakeTop,
                    QuizSetId = x.QuizSetId
                }).ToList();

            transaction = await _unitOfWork.OpenTransactionAsync(cancellationToken);
            
            await _unitOfWork.Events.AddAsync(newEvent, cancellationToken);
            if (quizSessions is not null)
            {
                await _unitOfWork.QuizSessions.AddRangeAsync(quizSessions, cancellationToken);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var query = new GetOwnEventQuery { EventId = newEvent.Id };
            var fullEvent = await _mediator.Send(query, cancellationToken);
            var responseData = fullEvent.Data;
            
            await _eventPublishService.PublishEventUpdatedEventAsync(fullEvent.Data, cancellationToken);
            response.ToSuccessResponse(fullEvent.Data);
        }
        catch (Exception e)
        {
            _logger.LogError($"{methodName} {e.Message}");
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            response.ToInternalErrorResponse();
        }

        return response;
    }
}