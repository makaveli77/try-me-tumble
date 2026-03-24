namespace TryMeTumble.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid WebsiteId { get; set; }
        public Website Website { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}