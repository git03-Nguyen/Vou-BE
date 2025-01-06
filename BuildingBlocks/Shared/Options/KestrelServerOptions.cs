namespace Shared.Options;

public class KestrelServerOptions
{
    public const string OptionName = "KestrelServerOptions";
    public int RequestHeadersTimeout { get; set; }
    public int KeepAliveTimeout { get; set; }
    public int MaxRequestBodySize { get; set; }
}