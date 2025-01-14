using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Queries.CounterPartQueries.GetAllCounterParts;

public class GetAllCounterPartsQuery : IRequest<BaseResponse<GetAllCounterPartsResponse>>
{
    
}

public class GetAllCounterPartsResponse
{
    public List<CounterPartDto> CounterParts { get; set; }
}