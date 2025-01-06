namespace AuthServer.Data.Entities;

public class ProfileLinked {
    public bool BasicAuth { get; set; }
    public bool Google { get; set; }
    public ProfileLinked ShallowCopy() => (ProfileLinked) MemberwiseClone();
}