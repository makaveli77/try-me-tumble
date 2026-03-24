namespace TryMeTumble.Application.DTOs
{
    public class SavedWebsiteDto
    {
        public Guid Id { get; set; }
        public Guid WebsiteId { get; set; }
        public WebsiteResponseDto Website { get; set; } = null!;
        public DateTime SavedAt { get; set; }
    }
}