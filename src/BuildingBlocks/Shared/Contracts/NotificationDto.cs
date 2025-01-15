namespace Shared.Contracts;

public class NotificationDto
{
    public string Id { get; set; }
    public string PlayerId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}