namespace Shared.Services.CachingServices;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken, TimeSpan? ttl = null);
    Task RemoveAsync(string key, CancellationToken cancellationToken);
}