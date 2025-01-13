namespace GameService.Helpers;

public static class TimeHelpers
{
    public static DateTime GetNextMonday()
    {
        var today = DateTime.Today;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntilMonday);
    }
    
    public static DateTime GetNearestLastMonday()
    {
        var today = DateTime.Today;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntilMonday - 7);
    }
}