using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shared.Filters;
using Shared.Options;

namespace Shared.StartupRegistrations;

public static class SwaggerDocsRegistrations
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var servicesOptions = configuration.GetSection(ServicesOptions.OptionName).Get<ServicesOptions>();
        var currentService = servicesOptions?.Name ?? "Service";
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{currentService} APIs Document",
                Version = "v1",
                Description = "This is the api detailed description for the project.\n Accepted value for {apiVersion} : [1]",
                Contact = new OpenApiContact
                {
                    Name = "Author",
                    Email = "nguyendinhanhvlqt@gmail.com"
                }
            });

            options.SchemaFilter<SwaggerEnumFilter>();
            options.CustomSchemaIds(type => type.ToString());

            var filePath = Path.Combine(AppContext.BaseDirectory, $"{currentService}.xml");
            options.IncludeXmlComments(filePath, true);

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "Enter JWT Bearer token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.CustomSchemaIds(type => type.FullName);

            options.AddSignalRSwaggerGen();
        });
        
        return services;
    }

    public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsProduction()) return app;
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}