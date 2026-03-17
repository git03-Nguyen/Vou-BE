namespace Shared.Domain;

public abstract class BaseEntity : IBaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsDeleted { get; set; } = false;
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
}