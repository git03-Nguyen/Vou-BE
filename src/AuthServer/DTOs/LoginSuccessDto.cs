namespace AuthServer.DTOs;

public class LoginSuccessDto
{
    public string AccessToken { get; set; }
    public long LifeTime { get; set; }
}