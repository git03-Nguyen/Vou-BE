namespace AuthServer.Data.Models;

public class ProfileLinked
{
    public bool BasicAuth { get; set; } = true;
    public bool Google { get; set; } = false;
    public ProfileLinked ShallowCopy() => (ProfileLinked) MemberwiseClone();
}