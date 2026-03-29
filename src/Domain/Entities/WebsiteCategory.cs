namespace TryMeTumble.Domain.Entities;

public class WebsiteCategory
{
    public Guid WebsiteId { get; set; }
    public Website Website { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}