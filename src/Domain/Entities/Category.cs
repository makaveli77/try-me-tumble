namespace TryMeTumble.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<WebsiteCategory> WebsiteCategories { get; set; } = new List<WebsiteCategory>();
    }
}