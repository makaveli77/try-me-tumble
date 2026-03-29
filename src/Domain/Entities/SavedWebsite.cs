namespace TryMeTumble.Domain.Entities;

public class SavedWebsite
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid WebsiteId { get; set; }
    public Website Website { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}