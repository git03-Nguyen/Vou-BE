using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] validatorAssemblies)
    {
        return services.Scan(scan => scan
            .FromAssemblies(validatorAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}