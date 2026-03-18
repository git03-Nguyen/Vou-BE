using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;
using Shared.Validation;

namespace Shared.StartupRegistrations;

public static class ApiBehaviorRegistrations
{
    public static IServiceCollection ConfigureDefaultValidationResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => new ValidationError { Message = x.ErrorMessage })
                    .ToList();

                var response = new ValidationBaseResponse
                {
                    Status = HttpStatusCode.BadRequest.ToInt(),
                    Message = "One or more validation errors occurred",
                    Data = errors
                };

                return response.ToObjectResult();
            };
        });

        return services;
    }
}
