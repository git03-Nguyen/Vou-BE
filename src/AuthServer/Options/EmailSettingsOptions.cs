namespace AuthServer.Options;

public class EmailSettingsOptions
{
    public const string OptionName = "EmailSettings";
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool UseSsl { get; set; }
}