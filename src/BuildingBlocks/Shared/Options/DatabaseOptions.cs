namespace Shared.Options;

public class DatabaseOptions
{
    public static readonly string OptionName = "Database";
    public string DefaultSchema { get; init; } = "auth";
    public string ConnectionString { get; init; } = string.Empty;
}