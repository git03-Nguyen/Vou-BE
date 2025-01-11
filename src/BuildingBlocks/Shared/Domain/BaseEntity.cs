namespace Shared.Domain;

public abstract class BaseEntity : IBaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsDeleted { get; set; } = false;
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public DateTime? ModifiedDate { get; set; } = DateTime.Now;
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
}