namespace AuthServer.Options;

public class AuthServerOption
{
    public static readonly string OptionName = "AuthServer";
    public string AuthUrl { get; init; } = "http://localhost:5001";
    public string Secret { get; init; } = "my_auth_server_secret";
}