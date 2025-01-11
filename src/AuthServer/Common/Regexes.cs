namespace AuthServer.Common;

public static class Regexes
{
    public static readonly string VALID_PASSWORD = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).\S{7,15}$";
    public static readonly string VALID_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
    public static readonly string VALID_PHONE = @"^([0-9]{10,15})$";
    public static readonly string VALID_USERNAME = @"^[a-zA-Z0-9]{3,20}$";
    public static readonly string VALID_FULLNAME = @"^[\p{L}0-9'. ]{1,50}$";
    public static readonly string VALID_COUNTERPART_NAME = @"^[\p{L}0-9'. ]{1,50}$";
    public static readonly string VALID_COUNTERPART_FIELD = @"^[\p{L}0-9'. ]{1,50}$";
    public static readonly string VALID_ADDRESS_TEXT = @"^[\p{L}0-9'., ]{1,250}$";
    public static readonly string VALID_FACEBOOK_URL = @"^((https?:\/\/)?(www\.)?)?facebook\.com\/[a-zA-Z0-9.]+$";
}