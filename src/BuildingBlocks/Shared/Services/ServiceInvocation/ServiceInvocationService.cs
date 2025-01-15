using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace Shared.Services.ServiceInvocation;

public class ServiceInvocationService : IServiceInvocationService
{
    private readonly ILogger<ServiceInvocationService> _logger;
    private readonly DaprClient _daprClient;
    public ServiceInvocationService(ILogger<ServiceInvocationService> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<TResponse?> InvokeServiceAsync<TRequest, TResponse>(HttpMethod httpMethod, string appId, string methodName, TRequest requestData, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ServiceInvocationService)}.{nameof(InvokeServiceAsync)} AppId: {appId}, MethodName: {methodName}, HttpMethod: {httpMethod} =>";
        _logger.LogInformation(functionName);
        
        try
        {
            var response = await _daprClient
                .InvokeMethodAsync<TRequest, TResponse>
                (
                    httpMethod,
                    appId,
                    methodName,
                    requestData,
                    cancellationToken
                );
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{functionName} Has error: {ex.Message}");
            return default;
        }
    }
}