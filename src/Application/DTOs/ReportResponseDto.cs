namespace TryMeTumble.Application.DTOs;

public class ReportResponseDto
{
    public Guid Id { get; set; }
    public Guid WebsiteId { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Username { get; set; }
    public required string Reason { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
}
