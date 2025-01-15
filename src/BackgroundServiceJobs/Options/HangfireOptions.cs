namespace BackgroundServiceJobs.Options;

public class HangfireOptions
{
    public const string OptionName = "Hangfire";
    public string ConnectionString { get; set; }
    public long SecondsBeforeToNotifyEvent { get; set; } = 10;
}