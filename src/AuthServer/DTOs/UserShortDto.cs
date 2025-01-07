namespace AuthServer.DTOs;

public class UserShortDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string AvatarUrl { get; set; }
    public string Role { get; set; }
}