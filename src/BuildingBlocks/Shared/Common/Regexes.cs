namespace Shared.Common;

public static class Regexes
{
    public static readonly string VALID_PASSWORD = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).\S{7,15}$";
    public static readonly string VALID_EMAIL = "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$";
    public static readonly string VALID_PHONE = @"^([0-9]{10,15})$";
    public static readonly string VALID_USERNAME = @"^[a-zA-Z0-9]{3,20}$";
    public static readonly string VALID_FULLNAME = @"^[\p{L}0-9'. ]{1,50}$";
    public static readonly string VALID_COUNTERPART_FIELD = @"^[\p{L}0-9'. ]{1,50}$";
    public static readonly string VALID_ADDRESS_TEXT = @"^[\p{L}0-9'., ]{1,250}$";
    public static readonly string VALID_FACEBOOK_URL = @"^https?://.*facebook";
    public static readonly string VALID_URL = @"^https?://";
    
}