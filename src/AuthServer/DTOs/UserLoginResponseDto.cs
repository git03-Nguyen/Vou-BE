namespace AuthServer.DTOs;

public class UserLoginResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long ExpiresIn { get; set; }
    public UserFullProfileDto FullProfile { get; set; }
}