namespace GameService.Extensions;

public static class CustomStringExtensions
{
    public static string ToMaskedUserName(this string userName)
    {
        // Hidden 3 first chars
        if (string.IsNullOrEmpty(userName))
        {
            return string.Empty; // Return an empty string for null or empty input.
        }

        if (userName.Length <= 3)
        {
            // Mask the entire username if it's 3 or fewer characters long.
            return new string('*', userName.Length);
        }

        // Replace the first three characters with asterisks.
        return new string('*', 3) + userName.Substring(3);
    } 
}