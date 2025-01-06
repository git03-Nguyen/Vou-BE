namespace AuthServer.Options;

public class DbSettingsOption
{
    public static readonly string OptionName = "DbSettings";
    public string ConnectionString { get; init; } = string.Empty;
}