using AuthServer.Data.Models;

namespace AuthServer.DTOs;

public class UserDetailDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string AvatarUrl { get; set; }
    public string Role { get; set; }
    
    public ProfileLinked ProfileLinked { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    
    public bool IsBlocked { get; set; }
    public DateTime? BlockedDate { get; set; }
}