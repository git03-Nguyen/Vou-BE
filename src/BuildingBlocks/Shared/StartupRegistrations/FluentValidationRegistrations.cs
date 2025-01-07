using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validation;

namespace Shared.StartupRegistrations;

public static class FluentValidationRegistrations
{
    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services, params Assembly[] validatorAssemblies)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services.Scan(scan => scan
            .FromAssemblies(validatorAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}