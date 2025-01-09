using Shared.Data;
using Shared.Domain;

namespace GameService.Data.Models.SyncModels;

public class Event : SyncEntity
{
    public string Id { get; set; }
    public string CounterPartId { get; set; }
    public string Name { get; set; }
    
}