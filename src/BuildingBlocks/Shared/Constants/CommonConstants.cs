namespace Shared.Constants;

public static class CommonConstants
{
    public const string SystemName = "System";
    
    public enum Roles
    {
        Admin = 0,
        Counterpart = 1,
        User = 2
    }
    
    public const string GOOGLE_AUTH = "https://accounts.google.com";
    public static readonly string DefaultAvatar = "https://storageaccwct.blob.core.windows.net/wct-blobstorage/UserDefaultAvatar.png";
}