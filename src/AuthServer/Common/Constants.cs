namespace AuthServer.Common;

public static class Constants
{
    public const string IdentityServer4RequestToken = "connect/token";
    public const string IdentityServer4RevokeToken = "connect/revocation";
    public const string PasswordNone = "NONE";
    public const string PhoneNumberDefault = "0000000000";
    public const string EmailConfirmation = "EmailConfirmationCustom";
    public const string ResetPassword = "ResetPasswordCustom";
    public const int PasswordMaxLength = 16;
    public const int PasswordMinLength = 8;
    public const string DefaultLastName = "1";

    public const string SYSTEM = "SYSTEM";
    public const string ADMIN = "ADMIN";
    public const string COUNTERPART = "COUNTERPART";
    public const string PLAYER = "PLAYER";
    
    public const string DefaultAvatarUrl = "https://placehold.co/600x400?text=Hello+World";
    public const string DefaultCounterPartName = "Anonymous CounterPart";
}