namespace Shared.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsLocalEnvironment()
    {
        var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(currentEnv, "localhost", StringComparison.OrdinalIgnoreCase);
    }
}
