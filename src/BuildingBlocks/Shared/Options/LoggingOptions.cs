namespace Shared.Options
{
    public class LoggingOptions
    {
        public const string OptionName = "Logging";
        public bool ConsoleEnabled { get; init; }
        public ElkOptions Elk { get; init; }
    }
    
    public class ElkOptions
    {
        public bool Enabled { get; set; }
        public string Uri { get; set; }
    }
}
