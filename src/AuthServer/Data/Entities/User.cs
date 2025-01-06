using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Shared.Data.Entities;

namespace AuthServer.Data.Entities;

public class User : IdentityUser, IBaseEntity
{
    public override string Id { get; set; }
    public string FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    
    [Column(TypeName = "jsonb")]
    public ProfileLinked ProfileLinked { get; set; }
}