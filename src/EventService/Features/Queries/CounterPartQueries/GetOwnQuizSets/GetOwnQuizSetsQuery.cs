using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetOwnQuizSets;

public class GetOwnQuizSetsQuery : IRequest<BaseResponse<GetOwnQuizSetsQueryResponse>>
{
}

public class GetOwnQuizSetsQueryResponse
{
    public List<QuizSetDto> QuizSets { get; set; }
}