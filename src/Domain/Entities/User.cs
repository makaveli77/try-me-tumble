using System.ComponentModel.DataAnnotations;

namespace TryMeTumble.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }

    public ICollection<Website> SubmittedWebsites { get; set; } = new List<Website>();
    public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
    public ICollection<SavedWebsite> SavedWebsites { get; set; } = new List<SavedWebsite>();
}
