namespace TryMeTumble.Application.DTOs;

public class UpvotedWebsiteDto
{
    public Guid Id { get; set; }
    public Guid WebsiteId { get; set; }
    public WebsiteResponseDto Website { get; set; } = null!;
    public DateTime UpvotedAt { get; set; }
}
