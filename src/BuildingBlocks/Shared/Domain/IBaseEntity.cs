namespace Shared.Domain;

public interface IBaseEntity
{
    public string Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
}