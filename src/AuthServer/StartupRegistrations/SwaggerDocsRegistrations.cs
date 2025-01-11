using Microsoft.OpenApi.Models;
using Shared.Filters;
using Shared.Options;

namespace AuthServer.StartupRegistrations;

public static class SwaggerDocsRegistrations
{
    public static IServiceCollection ConfigureSwaggerDocs(this IServiceCollection services, IConfiguration configuration)
    {
        var servicesOptions = configuration.GetSection(ServicesOptions.OptionName).Get<ServicesOptions>();
        var authOptions = configuration.GetSection(AuthenticationOptions.OptionName).Get<AuthenticationOptions>();
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

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authOptions!.Authority),
                        TokenUrl = new Uri(authOptions.Authority + "/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            {"service_scope", "API for service"}
                        }
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "oauth2",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>{ "BackOfficeAPI" }
                }
            });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.CustomSchemaIds(type => type.FullName);

            options.AddSignalRSwaggerGen();
        });
        
        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsProduction()) return app;
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}