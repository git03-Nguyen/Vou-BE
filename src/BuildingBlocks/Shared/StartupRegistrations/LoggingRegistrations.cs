using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Shared.Options;
using LoggingOptions = Shared.Options.LoggingOptions;

namespace Shared.StartupRegistrations;

public static class LoggingRegistrations
{
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            var serviceOptions = context.Configuration.GetSection(ServicesOptions.OptionName).Get<ServicesOptions>();
            var loggingOptions = context.Configuration.GetSection(LoggingOptions.OptionName).Get<LoggingOptions>();
            var serviceName = serviceOptions?.Name ?? "Unknown.Service";
            var environmentName = context.HostingEnvironment.EnvironmentName;

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("Environment", environmentName);
            
            // Console logging
            if (loggingOptions!.ConsoleEnabled)
            {
                loggerConfiguration.WriteTo.Console();
            }

            // Elasticsearch logging
            if (loggingOptions.Elk.Enabled && !string.IsNullOrWhiteSpace(loggingOptions.Elk.Uri))
            {
                loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(loggingOptions.Elk.Uri))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                    TemplateName = $"application-logs-{environmentName}",
                    OverwriteTemplate = true,
                    IndexFormat = $"{environmentName.ToLower()}-{DateTime.UtcNow:yyyy.MM.dd}",
                    DetectElasticsearchVersion = true,
                    RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
                    TypeName = null,
                    BatchAction = ElasticOpType.Create,
                    FailureCallback = (e, ex) => Console.WriteLine("Unable to submit event " + e.MessageTemplate + " Error: " + ex.Message),
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog | EmitEventFailureHandling.RaiseCallback
                });
            }
        });

        return hostBuilder;
    }
}