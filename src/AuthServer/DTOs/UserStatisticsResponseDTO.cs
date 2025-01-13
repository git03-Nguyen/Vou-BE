namespace AuthServer.DTOs;

public class UserStatisticsResponseDto
{
    public int TotalUsers { get; set; }
    public int TotalActiveUsers { get; set; }
    public int TotalPlayers { get; set; }
    public int TotalCounterParts { get; set; }

}