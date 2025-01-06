namespace AuthServer.Constants;

public static class Regex
{
    public static readonly string VALID_PASSWORD = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).\S{7,15}$";
    public static readonly string VALID_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
    public static readonly string VALID_PHONE = @"^([0-9]{10,15})$";
}