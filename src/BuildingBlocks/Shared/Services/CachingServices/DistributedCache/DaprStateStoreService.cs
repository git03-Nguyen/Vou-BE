using Dapr.Client;

namespace Shared.Services.CachingServices.DistributedCache;

public class DaprStateStoreService : IDaprStateStoreService
{
    private readonly DaprClient _daprClient;
    private readonly string _stateStoreName;
    public DaprStateStoreService(DaprClient daprClient, string stateStoreName = "statestore")
    {
        _daprClient = daprClient;
        _stateStoreName = stateStoreName;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        return await _daprClient.GetStateAsync<T>(_stateStoreName, key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        var options = new StateOptions
        {
            Consistency = ConsistencyMode.Strong,
            Concurrency = ConcurrencyMode.FirstWrite,
        };
        
        var metadata = ttl.HasValue
            ? new Dictionary<string, string>
            {
                { "ttlInSeconds", ((int)ttl.Value.TotalSeconds).ToString() }
            }
            : null;
        
        await _daprClient.SaveStateAsync(_stateStoreName, key, value, options, metadata);
    }

    public async Task RemoveAsync(string key)
    {
        await _daprClient.DeleteStateAsync(_stateStoreName, key);
    }
}