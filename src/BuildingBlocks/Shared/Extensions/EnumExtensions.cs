namespace Shared.Extensions;

public static class EnumExtensions
{
    public static int ToInt(this Enum value)
    {
        return Convert.ToInt32(value);
    }
}