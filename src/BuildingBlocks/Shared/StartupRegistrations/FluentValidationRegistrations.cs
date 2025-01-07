using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.StartupRegistrations;

public static class FluentValidationRegistrations
{
    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services, params Assembly[] validatorAssemblies)
    {
        return services.Scan(scan => scan
            .FromAssemblies(validatorAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}