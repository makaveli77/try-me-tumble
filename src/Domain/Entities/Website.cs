using System.ComponentModel.DataAnnotations;

namespace TryMeTumble.Domain.Entities
{
    public class Website
    {
        public Guid Id { get; set; }
        [Required]
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public Guid SubmittedById { get; set; }
        public User SubmittedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WebsiteCategory> WebsiteCategories { get; set; } = new List<WebsiteCategory>();
        public ICollection<WebsiteTag> WebsiteTags { get; set; } = new List<WebsiteTag>();
        public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
        public ICollection<SavedWebsite> SavedWebsites { get; set; } = new List<SavedWebsite>();
    }
}
