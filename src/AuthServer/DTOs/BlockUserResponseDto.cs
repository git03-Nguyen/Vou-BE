namespace AuthServer.DTOs;

public class BlockUserResponseDto
{
    public bool IsBlocked { get; set; }
    public DateTime BlockedDate { get; set; }
}