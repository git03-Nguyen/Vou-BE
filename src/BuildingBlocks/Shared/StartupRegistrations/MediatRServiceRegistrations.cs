using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.StartupRegistrations;

public static class MediatRServiceRegistrations
{
    public static IServiceCollection ConfigureMediatRService(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        return services;
    }
}