namespace Shared.Domain;

public abstract class SyncEntity : ISyncEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}