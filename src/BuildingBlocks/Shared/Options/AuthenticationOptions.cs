namespace Shared.Options;

public class AuthenticationOptions
{
    public static readonly string OptionName = "Authentication";
    public string Authority { get; init; } = "http://localhost:5001";
    public int TokenLifeTime { get; init; } = 30000;
    public string Secret { get; init; } = "my_auth_server_secret";
    public string ProviderKey { get; init; } = "IdentityApiKey";
}