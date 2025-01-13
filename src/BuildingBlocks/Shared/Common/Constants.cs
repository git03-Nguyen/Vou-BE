namespace Shared.Common;

public static class Constants
{
    public const string SYSTEM = "SYSTEM";
    public const string ADMIN = "ADMIN";
    public const string COUNTERPART = "COUNTERPART";
    public const string PLAYER = "PLAYER";
    
    public enum Roles
    {
        ADMIN = 0,
        COUNTERPART = 1,
        PLAYER = 2
    }
    
    public const string GOOGLE_AUTH = "https://accounts.google.com";
    public const string DefaultAvatar = "https://storageaccwct.blob.core.windows.net/wct-blobstorage/UserDefaultAvatar.png";
    public const string DefaultGameImageUrl = "https://storageaccwct.blob.core.windows.net/wct-blobstorage/UserDefaultAvatar.png";
    
    public const string PubSubName = "pubsub";
    
    
}