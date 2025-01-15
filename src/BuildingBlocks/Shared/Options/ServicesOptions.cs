namespace Shared.Options;

public class ServicesOptions
{
    public const string OptionName = "Services";
    public string Name { get; set; }
    
    public AuthServerOptions AuthServer { get; set; } = new();
    public ApiGatewayOptions ApiGateway { get; set; } = new();
    public EventServiceOptions EventService { get; set; } = new();
    public GameServiceOptions GameService { get; set; } = new();
    public BackgroundServiceJobsOptions BackgroundServiceJobs { get; set; } = new();
}

public class AuthServerOptions
{
    public string Name { get; set; } = "AuthServer";
}

public class ApiGatewayOptions
{
    public string Name { get; set; } = "ApiGateway";
}

public class EventServiceOptions
{
    public string Name { get; set; } = "EventService";
}

public class GameServiceOptions
{
    public string Name { get; set; } = "GameService";
}

public class BackgroundServiceJobsOptions
{
    public string Name { get; set; } = "BackgroundServiceJobs";
}

