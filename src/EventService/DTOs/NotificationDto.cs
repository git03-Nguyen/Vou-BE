namespace EventService.DTOs;

public class NotificationDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
}