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
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #4CAF50;'>Welcome to Vou App!</h2>
            
            <p>Thank you for registering with us. We are excited to have you on board!</p>
            
            <p style='font-size: 1.2em;'>
                <strong>Your OTP activation code is:</strong>
                <span style='color: #FF5722; font-weight: bold;'>{otp}</span>
            </p>

            <p>Please use this code to complete your registration. The code is valid for <strong>5 minutes</strong>.</p>
            
            <hr style='border: none; border-top: 1px solid #DDD; margin: 20px 0;' />

            <p style='font-size: 0.9em; color: #555;'>
                If you did not request this code, please ignore this email or contact our support team.
            </p>
            
            <p>Thank you for choosing Vou App!</p>
            
            <p style='margin-top: 20px; font-size: 0.9em; color: #888;'>
                Best regards, <br />
                <strong>The Vou App Team</strong>
            </p>
        </div>
    ";
    }
}