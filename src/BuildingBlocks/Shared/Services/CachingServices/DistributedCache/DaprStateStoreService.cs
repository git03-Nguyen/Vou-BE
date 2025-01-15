using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace Shared.Services.CachingServices.DistributedCache;

public class DaprStateStoreService : IDaprStateStoreService
{
    private readonly DaprClient _daprClient;
    private readonly string _stateStoreName;
    private readonly ILogger<DaprStateStoreService> _logger;
    public DaprStateStoreService(DaprClient daprClient, ILogger<DaprStateStoreService> logger, string stateStoreName = "statestore")
    {
        _daprClient = daprClient;
        _logger = logger;
        _stateStoreName = stateStoreName;
    }
    
    private static readonly StateOptions StateOptions = new()
    {
        Consistency = ConsistencyMode.Strong,
        Concurrency = ConcurrencyMode.FirstWrite,
    };
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        return await _daprClient.GetStateAsync<T>(_stateStoreName, key, cancellationToken: cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken,  TimeSpan? ttl = null)
    {
        var methodName = $"{nameof(DaprStateStoreService)}.{nameof(SetAsync)} Key: {key}, Value: {value}, TTL: {ttl} =>";
        _logger.LogInformation(methodName);
        
        var metadata = ttl.HasValue
            ? new Dictionary<string, string>
            {
                { "ttlInSeconds", ((int)ttl.Value.TotalSeconds).ToString() }
            }
            : null;

        try
        {
            await _daprClient.SaveStateAsync(_stateStoreName, key, value, StateOptions, metadata, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        var methodName = $"{nameof(DaprStateStoreService)}.{nameof(RemoveAsync)} Key: {key} =>";
        _logger.LogInformation(methodName);
        
        try
        {
            await _daprClient.DeleteStateAsync(_stateStoreName, key, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
        }
    }
}