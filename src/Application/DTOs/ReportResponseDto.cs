namespace TryMeTumble.Application.DTOs
{
    public class ReportResponseDto
    {
        public Guid Id { get; set; }
        public Guid WebsiteId { get; set; }
        public string WebsiteUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}