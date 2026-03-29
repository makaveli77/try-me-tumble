namespace TryMeTumble.Domain.Entities;

public class WebsiteTag
{
    public Guid WebsiteId { get; set; }
    public Website Website { get; set; } = null!;
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}