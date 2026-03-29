namespace TryMeTumble.Application.DTOs;

public class WebsiteDto
{
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public Guid? CategoryId { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}
