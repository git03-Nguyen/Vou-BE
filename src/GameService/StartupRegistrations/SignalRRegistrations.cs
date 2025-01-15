using GameService.SignalRHubs.QuizGame;
using MessagePack;
using Microsoft.AspNetCore.Http.Connections;

namespace GameService.StartupRegistrations;

public static class SignalRRegistrations
{
    public static IServiceCollection ConfigureSignalR(this IServiceCollection services)
    {
        services.AddSignalR().AddHubOptions<QuizGameHub>(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.MaximumParallelInvocationsPerClient = 2;
            options.StreamBufferCapacity = 10;
        })
        .AddMessagePackProtocol(options =>
        {
            options.SerializerOptions = MessagePackSerializerOptions.Standard
                .WithAllowAssemblyVersionMismatch(true)
                .WithOmitAssemblyVersion(true)
                .WithSecurity(MessagePackSecurity.UntrustedData)
                .WithCompression(MessagePackCompression.Lz4Block);
        });
            
        return services;
    }
    
    public static void MapSignalRHubs(this IEndpointRouteBuilder app)
    {
        app.MapHub<QuizGameHub>("/ws/QuizSession", options =>
        {
            options.Transports = HttpTransportType.WebSockets;
            options.CloseOnAuthenticationExpiration = true;
            options.ApplicationMaxBufferSize = 65536;
            options.TransportMaxBufferSize = 65536;
            options.TransportSendTimeout = TimeSpan.FromSeconds(10);
            options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(5);
        });
    }
}