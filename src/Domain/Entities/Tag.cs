namespace TryMeTumble.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<WebsiteTag> WebsiteTags { get; set; } = new List<WebsiteTag>();
    }
}