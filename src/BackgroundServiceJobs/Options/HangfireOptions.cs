namespace PaymentService.Options;

public class HangfireOptions
{
    public const string OptionName = "Hangfire";
    public string ConnectionString { get; set; }
}