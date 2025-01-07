namespace Shared.Options;

public class AuthenticationOptions
{
    public static readonly string OptionName = "Authentication";
    public string Authority { get; init; } = "http://localhost:5001";
    public string Secret { get; init; } = "my_auth_server_secret";
}