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
    
    public const string DefaultAvatarUrl = "https://placehold.co/600x400?text=Hello+World";
    public const string DefaultCounterPartName = "Anonymous CounterPart";
    
    public const string ActivateSubjectEmail = "OTP Code for Vou App Activation";
    public static string GetOtpActivateAccountMessage(string otp)
    {
        return $@"
        Thank you for registering with Vou App!
        
        Your OTP activation code is: {otp}
        Please use this code to complete your registration.

        Note: This code will expire in 5 minutes.

        Best regards,
        The Vou App Team
    ";
    }
}