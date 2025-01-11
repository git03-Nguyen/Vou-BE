namespace Shared.Domain;

public interface ISyncEntity
{
    public string Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}