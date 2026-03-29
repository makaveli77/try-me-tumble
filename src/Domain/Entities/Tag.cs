namespace TryMeTumble.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<WebsiteTag> WebsiteTags { get; set; } = new List<WebsiteTag>();
}