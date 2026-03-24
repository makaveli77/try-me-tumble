namespace TryMeTumble.Application.DTOs
{
    public class WebsiteDto
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}