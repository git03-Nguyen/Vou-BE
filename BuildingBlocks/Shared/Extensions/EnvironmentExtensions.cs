namespace Shared.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsLocalEnvironment()
    {
        var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return currentEnv == "localhost";
    }
}