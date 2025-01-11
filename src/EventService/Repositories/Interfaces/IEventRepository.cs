using EventService.Data.Models;
using Shared.Repositories;

namespace EventService.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
}