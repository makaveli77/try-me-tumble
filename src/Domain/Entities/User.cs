using System.ComponentModel.DataAnnotations;

namespace TryMeTumble.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public ICollection<Website> SubmittedWebsites { get; set; } = new List<Website>();
        public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
        public ICollection<SavedWebsite> SavedWebsites { get; set; } = new List<SavedWebsite>();
    }
}
