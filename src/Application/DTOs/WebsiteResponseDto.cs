namespace TryMeTumble.Application.DTOs
{
    public class WebsiteResponseDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<string> Tags { get; set; } = new List<string>();
    }
}