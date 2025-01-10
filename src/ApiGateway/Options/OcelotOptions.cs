namespace ApiGateway.Options;

public class OcelotOptions
{
    public const string OptionName = "Ocelot";
    public string Path { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
}