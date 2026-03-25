using EventService.DTOs;
using MediatR;
using Shared.Enums;
using Shared.Response;

namespace EventService.Features.Queries.AdminQueries.GetAllEvents;

public class GetAllEventsQuery : IRequest<BaseResponse<GetAllEventQueryResponse>>
{
    public string? CounterPartId { get; set; }
    public EventStatus? Status { get; set; }
    public bool? HasVoucherInventory { get; set; }
    public string? Search { get; set; }
}

public class GetAllEventQueryResponse
{
    public int TotalCount { get; set; }
    public List<AdminEventDto> Events { get; set; } = new();
}