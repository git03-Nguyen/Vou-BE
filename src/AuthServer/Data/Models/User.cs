using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Shared.Domain;

namespace AuthServer.Data.Models;

public class User : IdentityUser, IBaseEntity
{
    public override string Id { get; set; } = Guid.NewGuid().ToString();
    public string FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; }
    [Column(TypeName = "jsonb")]
    public ProfileLinked? ProfileLinked { get; set; }
    
    public bool IsBlocked { get; set; }
    public DateTime? BlockedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public DateTime? ModifiedDate { get; set; } = DateTime.Now;
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    
    //OTP activate account code and expired time
    public string? OtpActivateCode { get; set; }
    public DateTime? OtpActivateExpiredTime { get; set; }
    //OTP reset password code and expired time
    public string? OtpResetPasswordCode { get; set; }
    public DateTime? OtpResetPasswordExpiredTime { get; set; }
    
}