using BackgroundServiceJobs.Data.Models;
using Shared.Repositories;

namespace BackgroundServiceJobs.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
}