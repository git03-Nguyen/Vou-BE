namespace Shared.Services.ServiceInvocation;

public interface IServiceInvocationService
{
    Task<TResponse?> InvokeServiceAsync<TRequest, TResponse>(HttpMethod httpMethod, string appId, string methodName,
        TRequest requestData, CancellationToken cancellationToken);
}