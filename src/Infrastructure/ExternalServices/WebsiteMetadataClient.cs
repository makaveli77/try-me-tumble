using Microsoft.Extensions.Logging;
using TryMeTumble.Application.Interfaces;

namespace TryMeTumble.Infrastructure.ExternalServices
{
    public class WebsiteMetadataClient : IWebsiteMetadataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebsiteMetadataClient> _logger;

        public WebsiteMetadataClient(HttpClient httpClient, ILogger<WebsiteMetadataClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            // Best practice: configure resilient settings (timeouts) here or in Program.cs
        }

        public async Task<(string? Title, string? Description)> FetchMetadataAsync(string url)
        {
            try
            {
                // Asynchronous data flow to an external system
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                // Very rudimentary title parsing for architectural demonstration
                var title = ParseHtmlTag(content, "<title>", "</title>");
                
                _logger.LogInformation("Successfully fetched metadata for {Url}", url);
                return (title, null); // returning basic title
            }
            catch (Exception ex)
            {
                // Resilient API client: Handling external error logging gracefully
                _logger.LogError(ex, "Failed to fetch metadata for external URL: {Url}", url);
                return (null, null); // Return graceful fallback instead of crashing the API
            }
        }

        private static string? ParseHtmlTag(string html, string startTag, string endTag)
        {
            int startIndex = html.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1) return null;
            startIndex += startTag.Length;
            
            int endIndex = html.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);
            if (endIndex == -1) return null;

            return html.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}