namespace TryMeTumble.Application.DTOs;

public class WebsiteResponseDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public List<string> Tags { get; set; } = new List<string>();
}
