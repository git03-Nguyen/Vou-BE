using Microsoft.Extensions.Logging;

namespace Shared.Extensions;

public static class ExceptionExtensions
{
    public static void LogError(this Exception ex, string functionName, ILogger logger)
    {
        logger.LogError(ex, $"{functionName} Has error: {ex.Message}");
    }
}